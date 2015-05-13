using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  public enum Direction { Up, Down, Backward, Forward };

  public static GameObject LastCheckPoint { get; set; } 
  public static GameObject Body { get; set; }
  public static Player Actions { get; set; }
  public static bool Attacking { get; set; }
  public static bool Dead { get; set; }

  // Config
  public GameObject DeathParticles;
  public GameObject SwordStrike;

  public float MoveSpeed { get; set; }

  private GameObject swordTrail;
  private RaycastHit hit;
  private SpriteRenderer sprite;
  private Animator anim;
  private bool bouncing;
  private bool faceForward = true;
  private bool grounded;
  private bool openBackward;
  private bool openForward;
  private float alpha = 1f;
  private float coolDown;

  void Start() {
    Player.Dead = false;
    Player.Attacking = false;
    Player.Actions = gameObject.GetComponent<Player>();
    Player.Body = gameObject;

    GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, 0.5f);
    sprite = gameObject.GetComponent<SpriteRenderer>();
    anim = gameObject.GetComponent<Animator>();
  }

  void FixedUpdate() {
    float axis = Input.GetAxis("Horizontal");

    if (GameManager.LevelCompleted) return;

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

    if (GameManager.LevelCompleted) return;

    const int noEnemiesLayer = 1 << 8;
    openForward = Physics.Raycast(transform.position, Vector3.right, out hit, 0.2f, ~noEnemiesLayer);

    if (openForward && hit.collider.CompareTag("Breakable")) {
      hit.collider.GetComponent<Breakable>().Break();
    }

    openBackward = Physics.Raycast(transform.position, Vector3.left, out hit, 0.2f, ~noEnemiesLayer);
    grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f, ~noEnemiesLayer); 

    if (grounded || transform.position.y < 0.6f) {
      GetComponent<Collider>().isTrigger = false;
    }

    if (grounded && bouncing) {
      bouncing = false;
    }

    if (grounded && hit.collider.CompareTag("Fallable")) {
      hit.collider.GetComponent<Fallable>().Fall();
    }

    if (swordTrail) {
      var pos = new Vector3(transform.position.x + (faceForward ? 0.4f : -0.4f), transform.position.y, transform.position.z);
      swordTrail.transform.position = pos;
      swordTrail.transform.rotation = Quaternion.Euler(0, (faceForward ? -90f : 90f), 0);
    }

    if (Attacking && !faceForward & openBackward) {
      GetComponent<Rigidbody>().velocity = new Vector3(3f, -2f, 0f);
    }

    if (Attacking && faceForward & openForward) {
      GetComponent<Rigidbody>().velocity = new Vector3(-3f, -2f, 0f);
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
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x * 0.5f, GetComponent<Rigidbody>().velocity.y * 0.5f, 0f);
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

    if ((!openForward && h > 0 || !openBackward && h < 0) && !Player.Dead) {
      transform.Translate(new Vector3(h * 0.1f, 0, 0));
    }

    if (h < -0.01f && faceForward || h > 0.01f && !faceForward) {
      faceForward = !faceForward;
      transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
  }

  public void Jump () {
    if (grounded) {
      GetComponent<Rigidbody>().velocity = new Vector3(0f, 5.0f, 0f);
      GetComponent<Collider>().isTrigger = true;
    }
  }

  public void Attack () {
    if (coolDown.Equals(0f) && !Player.Dead) {
      Attacking = true;
      anim.SetBool("Attacking", true);

	  GameMusic.PlayAttack();
      GameManager.MakeBreakablesTrigger();

      const int noEnemiesLayer = 1 << 8;
      openForward = Physics.Raycast(transform.position, Vector3.right, out hit, 0.2f, ~noEnemiesLayer);
      openBackward = Physics.Raycast(transform.position, Vector3.left, out hit, 0.2f, ~noEnemiesLayer);

      if (Attacking && !faceForward & openBackward) {
        GetComponent<Rigidbody>().velocity = new Vector3(3f, -2f, 0f);
      } else if (Attacking && faceForward & openForward) {
        GetComponent<Rigidbody>().velocity = new Vector3(-3f, -2f, 0f);
      } else {
        GetComponent<Rigidbody>().velocity = new Vector3(faceForward ? 10.0f : -10.0f, 0f, 0f);
      }

      coolDown = 1f;

      var pos = new Vector3(transform.position.x + (faceForward ? 0.4f : -0.4f), transform.position.y, transform.position.z);
      swordTrail = Instantiate(SwordStrike, pos, Quaternion.Euler(0, (faceForward ? -90f : 90f), 0)) as GameObject;
      Destroy(swordTrail, 1f);
    }
  }

  void OnCollisionEnter(Collision component) {
    if (component.gameObject.tag == "BadTouch") {
      Kill();
    }

    if (component.gameObject.tag == "Enemy" && !Attacking) {
      Kill();
    }

    if (component.collider.CompareTag("Breakable")) {
      component.collider.GetComponent<Breakable>().Break();
    }
  }

  void OnTriggerEnter(Component component) {
    if (component.CompareTag("CheckPoint")) {
      LastCheckPoint = component.gameObject;
      component.gameObject.SetActive(false);
    }

    if (component.CompareTag("ActionPoint")) {
      component.gameObject.SetActive(false);
    }

    if (component.CompareTag("BadTouch") || (component.CompareTag("Enemy") && !Attacking)) {
      Kill();
    }

    if (component.CompareTag("Fallable")) {
      GetComponent<Collider>().isTrigger = false;
      component.GetComponent<Fallable>().Fall();
    }

    if (component.CompareTag("Breakable")) {
      component.GetComponent<Breakable>().Break();
    }
  }

  void OnTriggerStay(Component component) {
    if (component.CompareTag("LevelPoint")) {
      GameManager.LevelCompleted = true;

      anim.SetBool("Attacking", false);
      anim.SetBool("Walking", true);

      if (transform.position.x < component.gameObject.transform.position.x) {
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 1f, 0f);
        float step = 2f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, component.gameObject.transform.position, step);
      } else {
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 5f, 0f);
      }
    }
  }

  public void Bounce(Direction direction) {
    if (bouncing) return;
    bouncing = true;

    if (direction == Direction.Backward) {
      GetComponent<Rigidbody>().velocity = new Vector3(faceForward ? -10f : 10f, 5f, 0f);
      GetComponent<Collider>().isTrigger = false;
    }
  }

  public void Kill() {
    Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
    Destroy(particles, 1f);

    GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
	GameMusic.PlayInjure();
	
    if (!Player.Dead) {
      StartCoroutine(Respawn());
    }
  }

  IEnumerator Respawn () {
    Player.Dead = true;
    GetComponent<Collider>().enabled = false;
    gameObject.GetComponent<Renderer>().enabled = false;
    LastCheckPoint.gameObject.GetComponent<CheckPoint>().Respawn();

    yield return new WaitForSeconds(1);

    alpha = 0f;
    Player.Dead = false;
    GetComponent<Collider>().enabled = true;
    transform.position = new Vector3(Player.LastCheckPoint.transform.position.x, Player.LastCheckPoint.transform.position.y, transform.position.z);
    sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, 0);
    gameObject.GetComponent<Renderer>().enabled = true;
  }
}
