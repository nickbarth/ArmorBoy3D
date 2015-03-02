using UnityEngine;
using System.Collections;

public class SlimeBoss : MonoBehaviour {
  public int lives;
  public bool dead;
  public GameObject DeathParticles;
  public float timer = 0;
  public bool forward = true;
  public bool grounded = true;
  public bool waiting = true;

  public GameObject enemy;

  void Update() {
    if (waiting) return;

    timer += Time.deltaTime;

    if (timer >= 2f) {
      grounded = false;
      rigidbody.velocity = new Vector3(forward ? -5f : 5f, 7f, 1f);
      timer = 0;
    }

    if (enemy.gameObject.transform.position.x < gameObject.transform.position.x) {
      forward = true;
      transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    } else {
      forward = false;
      transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Player") {
      waiting = false;
      Player player = collider.gameObject.GetComponent<Player>();
      Vector3 pos = collider.gameObject.transform.position;

      if (Player.Attacking) {
        lives -= 1;

        if (lives <= 0) {
          Kill();
        } else {
          player.Bounce(Player.Direction.Backward);
          Vector3 injury = new Vector3(pos.x, pos.y, pos.z);
          Object particles = Instantiate(DeathParticles, injury, Quaternion.identity);
          Destroy(particles, 1f);
        }
      } else if (!grounded) {
        player.Kill();
      } else {
        player.Bounce(Player.Direction.Backward);
      }
    }
  }

  void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "Player" && !grounded) {
      Player player = collision.gameObject.GetComponent<Player>();
      player.Kill ();
    } else {
      grounded = true;
    }
  }

  public void Kill() {
    if (!dead) {
      Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
      Destroy(particles, 1f);
      Destroy(gameObject);
      dead = true;
    }
  }
}
