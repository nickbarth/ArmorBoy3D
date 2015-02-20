using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public GameObject DeathParticles;
	public RaycastHit hit;
	public bool forwardHit;
	public bool backwardHit;
	public bool goingForward;
	public float moveSpeed;

	void Start () {
		goingForward = true;
		moveSpeed = 0.01f;
	}

	void Update () {
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.4f, Color.green);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 0.4f, Color.red);
		Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 0.4f, Color.blue);

		int noEnemiesLayer = 1 << 8;
		
		backwardHit = Physics.Raycast(transform.position, Vector3.right, out hit, 0.4f, ~noEnemiesLayer);
		forwardHit = Physics.Raycast(transform.position, Vector3.left, out hit, 0.4f, ~noEnemiesLayer);

		if ((goingForward && forwardHit) || (!goingForward && backwardHit)) {
			goingForward = !goingForward;
			transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}

		if (goingForward) { 
			transform.Translate (Vector3.right * moveSpeed);
		} else {
			transform.Translate (Vector3.left * moveSpeed);
		}
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player" && PlayerController.isAttacking()) {
			this.Kill();
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player" && PlayerController.isAttacking()) {
			this.Kill();
		}
	}
	
	void Kill () {
		Object particles = Instantiate(DeathParticles, transform.position, Quaternion.identity);
		Destroy(this.gameObject);
		Destroy(particles, 1f);
	}
}
