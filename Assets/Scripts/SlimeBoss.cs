using UnityEngine;
using System.Collections;

public class SlimeBoss : MonoBehaviour {
	public int lives;
	public bool dead;
	public GameObject DeathParticles;
	public float timer = 0;
	public bool forward = true;
		
	void Update() {
		timer += Time.deltaTime;
		
		if (timer >= 2f) {
			rigidbody.velocity = new Vector3(forward ? -5f : 5f, 7f, 1f);
			timer = 0;
		}
	}
	
	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player") {
			Player player = collider.gameObject.GetComponent<Player>();
			Vector3 pos = collider.gameObject.transform.position;
			
			if (Player.attacking) {
				lives -= 1;
				
				if (lives <= 0) {
					Kill();
				} else {
					player.Bounce(Player.Direction.Backward);
					Vector3 injury = new Vector3(pos.x, pos.y, pos.z);
					Object particles = Instantiate(DeathParticles, injury, Quaternion.identity);
					Destroy(particles, 1f);
				}
			} else {
				player.Kill();
			}
		}
		
		if (collider.gameObject.tag == "RoomEnd") {
			forward = false;
			transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
		}
	}
	
	
	public void Kill() {
		if (!dead) {
			Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
			Destroy(particles, 1f);

			collider.enabled = false;
			gameObject.renderer.enabled = false;
			rigidbody.useGravity = false;
			rigidbody.velocity = new Vector3(0f, 0f, 0f);
			dead = true;
		}
	}
}
