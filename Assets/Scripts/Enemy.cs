using UnityEngine;

public class Enemy : MonoBehaviour, IRespawnable {
  public bool Dead { get; set; }

  // Config
  public GameObject DeathParticles;
  public GameObject WaitObject;
  public float MoveSpeed;

  private Color myColor;
  private Quaternion rot;
  private RaycastHit hit;
  private Vector3 pos;
  private bool backwardHit;
  private bool fadeOut;
  private bool forwardHit;
  private bool goingForward = true;
  private bool hasGravity;
  private float alpha = 1f;

  private SpriteRenderer sprite; 

  void Start() {
    sprite = gameObject.GetComponent<SpriteRenderer>();
    hasGravity = rigidbody.useGravity;
    pos = transform.position;
    rot = transform.rotation;
    MoveSpeed = MoveSpeed.Equals(0f) ? 0.03f : MoveSpeed;
  }

  void Update() {
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.4f, Color.green);
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 0.4f, Color.red);
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 0.4f, Color.blue);

    if (fadeOut) {
      alpha -= Time.deltaTime * 2;
      sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, alpha);
      Respawn();
    } else if (alpha < 1f) {
      alpha += Time.deltaTime;
      sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, alpha);
    }

    if (Dead) return;
    if (WaitObject != null && WaitObject.activeSelf) return;

    const int noEnemiesLayer = 1 << 8;

    backwardHit = Physics.Raycast(transform.position, Vector3.right, out hit, 0.4f, ~noEnemiesLayer);
    forwardHit = Physics.Raycast(transform.position, Vector3.left, out hit, 0.4f, ~noEnemiesLayer);

    if ((goingForward && forwardHit) || (!goingForward && backwardHit)) {
      goingForward = !goingForward;
      transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    if (goingForward) { 
      transform.Translate (Vector3.right * MoveSpeed);
    } else {
      transform.Translate (Vector3.left * MoveSpeed);
    }
  }

  void OnTriggerEnter(Component component) {
    if (component.gameObject.tag == "BadTouch") {
      Kill();
    }

    if (component.gameObject.tag == "Player") {
      Kill();
    }

    if (component.gameObject.tag == "Fallable") {
      Kill();
    }
  }

  void OnCollisionEnter(Collision component) {
    if (component.gameObject.tag == "BadTouch") {
      Kill();
    }

    if (component.gameObject.tag == "Player") {
      Kill();
    }

    if (component.gameObject.tag == "Fallable") {
      Kill();
    }

    if (component.gameObject.tag == "RoomEnd") {
      goingForward = !goingForward;
      transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
  }

  public void Kill() {
    if (!Dead) {
      Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
      Destroy(particles, 1f);

      collider.enabled = false;
      gameObject.renderer.enabled = false;
      rigidbody.useGravity = false;
      rigidbody.velocity = new Vector3(0f, 0f, 0f);
      Dead = true;
    }
  }

  public void Respawn() {
    if (alpha > 0f) {
      fadeOut = true;
      return;
    }

    sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, 1f);
    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    transform.position = pos;
    transform.rotation = rot;

    Dead = false;
    collider.enabled = true;
    fadeOut = false;
    gameObject.renderer.enabled = true;
    goingForward = true;
    goingForward = true;
    rigidbody.useGravity = hasGravity;
  }
}
