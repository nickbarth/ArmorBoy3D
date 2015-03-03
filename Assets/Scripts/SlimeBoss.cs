using UnityEngine;

public class SlimeBoss : MonoBehaviour {
  public bool Dead { get; set; }

  // Config
  public GameObject DeathParticles;
  public int Lives;

  private float timer;
  private bool faceForward = true;
  private bool grounded = true;
  private bool waiting = true;

  void Update() {
    timer += Time.deltaTime;

    if (waiting && timer >= 2f) {
      rigidbody.velocity = new Vector3(0f, 1f, 1f);
      timer = 0;
      return;
    }

    if (timer >= 2f) {
      grounded = false;
      rigidbody.velocity = new Vector3(faceForward ? -5f : 5f, 7f, 1f);
      timer = 0;
    }

    if (Player.Body.transform.position.x < gameObject.transform.position.x) {
      faceForward = true;
      transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    } else {
      faceForward = false;
      transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
    }
  }

  void OnTriggerEnter(Component component) {
    if (component.gameObject.tag == "Player" && Player.Attacking) {
      Lives -= 1;

      if (Lives <= 0) Kill();
      else Injure();

      waiting = false;
    } else if (component.gameObject.tag == "Player") {
      Player.Actions.Kill();
    } else {
      grounded = true;
    }
  }

  void OnCollisionEnter(Collision component) {
    if (component.gameObject.tag == "Player" && !grounded) {
      Player.Actions.Kill();
    } else {
      grounded = true;
    }
  }

  public void Kill() {
    if (!Dead) {
      Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
      Destroy(particles, 1f);
      Destroy(gameObject);
      Dead = true;
    }
  }

  public void Injure() {
    var pos = Player.Body.transform.position;
    var injury = new Vector3(pos.x, pos.y, pos.z);

    float factor = 1.1f;
    var scale = gameObject.transform.localScale;
    gameObject.transform.localScale = new Vector3(scale.x / factor, scale.y / factor, scale.z / factor);

    Player.Actions.Bounce(Player.Direction.Backward);

    Object particles = Instantiate(DeathParticles, injury, Quaternion.identity);
    Destroy(particles, 1f);
  }
}
