using UnityEngine;
using System.Collections;

public class Fallable : MonoBehaviour {
	public GameObject particles;
	public GameObject waitObject;

	public bool falling;
	public bool constant;
	public float delay;

	private Vector3 pos;
	private Quaternion rot;
	private Object explosion;

	void Awake () {
		if (delay == 0) {
			delay = 2f;
		}

		falling = false;
		rigidbody.useGravity = false;
		pos = transform.position;
		rot = transform.rotation;
	}

	public void Update() {
		if (constant && !falling) {
			Fall();
		}
	}

	public void Fall() {
		if (waitObject != null && waitObject.activeSelf) return;
		falling = true;
		rigidbody.useGravity = true;
	}
	
	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "BadTouch") {
			Kill();
		}
	}

	
	void OnCollisionEnter(Collision collision) {
		Fall();

		if (collision.gameObject.tag == "Ground") {
			Kill ();
		}
	}

	void Kill() {
		if (explosion == null) {
			explosion = Instantiate (particles, transform.position, Quaternion.identity);
			Destroy (explosion, 1f);
		}
		StartCoroutine(Respawn ());
	}
	
	IEnumerator Respawn () {
		collider.enabled = false;
		gameObject.renderer.enabled = false;
		rigidbody.freezeRotation = true;
		transform.rotation = rot;
		rigidbody.velocity = new Vector3(0f, 0f, 0f);
		rigidbody.useGravity = false;
		transform.position = pos;
		yield return new WaitForSeconds(delay);	
		rigidbody.freezeRotation = false;
		gameObject.renderer.enabled = true;
		collider.enabled = true;
		falling = false;
	}
}
