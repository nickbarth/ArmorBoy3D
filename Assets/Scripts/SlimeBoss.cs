using UnityEngine;

public class SlimeBoss : MonoBehaviour, IRespawnable {
  public bool Dead { get; set; }

  // Config
  public GameObject Teleporter;
  public GameObject DeathParticles;
  public int Lives;

  private float timer;
  private bool faceForward = true;
  private bool grounded = true;
  private bool waiting = true;

  private Vector3 _pos;
  private Vector3 _scale;
  private bool reset;

  void Start() {
    _pos = transform.position;
    _scale = gameObject.transform.localScale;
  }

  void Update() {
    timer += Time.deltaTime;

    if (reset && timer > 1f) {
      Respawn();
      reset = false;
    }

    if (waiting && timer >= 2f) {
      GetComponent<Rigidbody>().velocity = new Vector3(0f, 2f, 0f);
      timer = 0;
      return;
    }

    if (timer >= 2f) {
      grounded = false;
      GetComponent<Rigidbody>().velocity = new Vector3(faceForward ? -5f : 5f, 7f, 1f);
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
      Injure();

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
      Teleporter.gameObject.transform.position = gameObject.transform.position;

      Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
      Destroy(particles, 1f);
      Destroy(gameObject);
      Dead = true;
    }
  }

  public void Injure() {
    var pos = Player.Body.transform.position;
    var injury = new Vector3(pos.x, pos.y, pos.z);
    
	GameMusic.PlayInjure();

    Lives -= 1;

    float factor = 1.1f;
    var scale = gameObject.transform.localScale;
    gameObject.transform.localScale = new Vector3(scale.x / factor, scale.y / factor, scale.z / factor);

    Player.Actions.Bounce(Player.Direction.Backward);

    Object particles = Instantiate(DeathParticles, injury, Quaternion.identity);
    Destroy(particles, 1f);

    if (Lives <= 0) Kill();
  }

  public void Respawn() {
    if (!reset) {
      timer = 0f;
      reset = true;
      return;
    }

    waiting = true;
    gameObject.transform.localScale = _scale;
    transform.position = _pos;
    Lives = 12;
  }
}
