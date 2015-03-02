using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  public enum Direction { Up, Down, Backward, Forward };

  public static GameObject LastCheckPoint { get; set; } 
  public static bool Attacking { get; set; }
  public static bool Dead { get; set; }

  // Configuration
  public GameObject DeathParticles;
  public GameObject SwordStrike;

  public float MoveSpeed { get; set; }

  private GameObject swordTrail;
  private RaycastHit hit;
  private SpriteRenderer sprite;
  private Animator anim;
  private bool bouncing;
  private bool faceRight = true;
  private bool grounded;
  private bool walledLeft;
  private bool walledRight;
  private float alpha = 1f;
  private float coolDown;

  void Start() {
    Player.Dead = false;
    Player.Attacking = false;

    renderer.castShadows = true;
    renderer.receiveShadows = true;

    rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 0.5f);
    sprite = gameObject.GetComponent<SpriteRenderer>();
    anim = gameObject.GetComponent<Animator>();
  }

  void FixedUpdate() {
    float axis = Input.GetAxis("Horizontal");

    if (GameManager.levelCompleted) return;

    if (!axis.Equals(0f) && !Player.Attacking) {
      Move (axis);
    } else if (!MoveSpeed.Equals(0f) && !Attacking) {
      Move (MoveSpeed);
    } else {
      anim.SetBool("Walking", false);
    }
  }

  void Update() {
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.4f, Color.green);
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 0.1f, Color.red);
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 0.1f, Color.blue);

    if (alpha < 1f) {
      alpha += Time.deltaTime * 2;
      sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, alpha);
    }

    if (GameManager.levelCompleted) return;

    const int noEnemiesLayer = 1 << 8;
    walledRight = Physics.Raycast(transform.position, Vector3.right, out hit, 0.2f, ~noEnemiesLayer);
    walledLeft = Physics.Raycast(transform.position, Vector3.left, out hit, 0.2f, ~noEnemiesLayer);

    grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f, ~noEnemiesLayer); 

    if (grounded && bouncing) {
      bouncing = false;
    }

    if (grounded && hit.collider.gameObject.tag == "Fallable") {
      hit.collider.gameObject.GetComponent<Fallable>().Fall();
    }

    if (swordTrail) {
      var pos = new Vector3(transform.position.x + (faceRight ? 0.4f : -0.4f), transform.position.y, transform.position.z);
      swordTrail.transform.position = pos;
      swordTrail.transform.rotation = Quaternion.Euler(0, (faceRight ? -90f : 90f), 0);
    }

    if (Attacking && !faceRight & walledLeft) {
      rigidbody.velocity = new Vector3(3f, -2f, 0f);
    }

    if (Attacking && faceRight & walledRight) {
      rigidbody.velocity = new Vector3(-3f, -2f, 0f);
    }

    if (Input.GetKey(KeyCode.UpArrow)) {
      Jump();
    }

    if (Input.GetKey(KeyCode.Space)) {
      Attack();
    }

    if (!coolDown.Equals(0f)) {
      coolDown += Time.deltaTime;
    }

    if (coolDown >= 1.5f) {
      if (Attacking) {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x * 0.5f, rigidbody.velocity.y * 0.5f, 0f);
      }

      anim.SetBool("Attacking", false);
      Attacking = false;
      GameManager.MakeBreakablesTrigger(false);
    }

    if (coolDown >= 2.0f) {
      coolDown = 0f;
    }
  }

  public void Move(float h) {
    if (Mathf.Abs(h) > 0.01f) {
      anim.SetBool("Walking", true);
    }

    if ((!walledRight && h > 0 || !walledLeft && h < 0) && !Player.Dead) {
      transform.Translate(new Vector3(h * 0.1f, 0, 0));
    }

    if (h < -0.01f && faceRight || h > 0.01f && !faceRight) {
      faceRight = !faceRight;
      transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
  }

  public void Jump () {
    if (grounded) {
      rigidbody.velocity = new Vector3(0f, 5.0f, 0f);
    }
  }

  public void Attack () {
    if (coolDown.Equals(0f) && !Player.Dead) {
      Attacking = true;
      anim.SetBool("Attacking", true);

      GameManager.MakeBreakablesTrigger();

      const int noEnemiesLayer = 1 << 8;
      walledRight = Physics.Raycast(transform.position, Vector3.right, out hit, 0.2f, ~noEnemiesLayer);
      walledLeft = Physics.Raycast(transform.position, Vector3.left, out hit, 0.2f, ~noEnemiesLayer);

      if (Attacking && !faceRight & walledLeft) {
        rigidbody.velocity = new Vector3(3f, -2f, 0f);
      } else if (Attacking && faceRight & walledRight) {
        rigidbody.velocity = new Vector3(-3f, -2f, 0f);
      } else {
        rigidbody.velocity = new Vector3(faceRight ? 10.0f : -10.0f, 0f, 0f);
      }

      coolDown = 1f;

      var pos = new Vector3(transform.position.x + (faceRight ? 0.4f : -0.4f), transform.position.y, transform.position.z);
      swordTrail = Instantiate(SwordStrike, pos, Quaternion.Euler(0, (faceRight ? -90f : 90f), 0)) as GameObject;
      Destroy(swordTrail, 1f);
    }
  }

  void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "BadTouch") {
      Kill();
    }

    if (collision.gameObject.tag == "Enemy" && !Attacking) {
      Kill();
    }
  }

  void OnTriggerEnter(Component collider) {
    if (collider.gameObject.tag == "CheckPoint") {
      LastCheckPoint = collider.gameObject;
      collider.gameObject.SetActive(false);
    }

    if (collider.gameObject.tag == "BadTouch" || (collider.gameObject.tag == "Enemy" && !Attacking)) {
      Kill();
    }
  }

  void OnTriggerStay(Component collider) {
    if (collider.gameObject.tag == "LevelPoint") {
      GameManager.levelCompleted = true;

      anim.SetBool("Attacking", false);
      anim.SetBool("Walking", true);

      if (transform.position.x < collider.gameObject.transform.position.x) {
        rigidbody.velocity = new Vector3(0f, 1f, 0f);
        float step = 2f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, collider.gameObject.transform.position, step);
      } else {
        rigidbody.velocity = new Vector3(0f, 5f, 0f);
      }
    }
  }

  public void Bounce(Direction direction) {
    if (bouncing) return;
    bouncing = true;

    if (direction == Direction.Backward) {
      rigidbody.velocity = new Vector3(faceRight ? -10f : 10f, 5f, 0f);
    }
  }

  public void Kill() {
    Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
    Destroy(particles, 1f);

    rigidbody.velocity = new Vector3(0f, 0f, 0f);

    if (!Player.Dead) {
      StartCoroutine(Respawn());
    }
  }

  IEnumerator Respawn () {
    Player.Dead = true;
    collider.enabled = false;
    gameObject.renderer.enabled = false;
    LastCheckPoint.gameObject.GetComponent<CheckPoint>().Respawn();

    yield return new WaitForSeconds(1);

    alpha = 0f;
    Player.Dead = false;
    collider.enabled = true;
    transform.position = new Vector3(Player.LastCheckPoint.transform.position.x, Player.LastCheckPoint.transform.position.y, transform.position.z);
    sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, 0);
    gameObject.renderer.enabled = true;
  }
}
