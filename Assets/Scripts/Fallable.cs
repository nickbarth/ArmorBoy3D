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
	
	private float delayTimer;

	void Awake () {
		if (delay == 0) {
			delay = 2f;
		}

		delayTimer = 0f;
		falling = false;
		rigidbody.useGravity = false;
		pos = transform.position;
		rot = transform.rotation;
	}

	public void Update() {
		if (constant && !falling && delayTimer >= delay) {
			Fall();
		} else if (constant && !falling) {
			delayTimer += Time.deltaTime;
		}
		
		if (transform.position.y < 100) {
			Respawn();
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
	
	IEnumerator Respawn() {
		rigidbody.useGravity = false;
		collider.enabled = false;
		gameObject.renderer.enabled = false;
		rigidbody.freezeRotation = true;
		transform.rotation = rot;
		rigidbody.velocity = new Vector3(0f, 0f, 0f);
		transform.position = pos;
		yield return new WaitForSeconds(delay);	
		rigidbody.freezeRotation = false;
		gameObject.renderer.enabled = true;
		collider.enabled = true;
		falling = false;
		delayTimer = 0f;
	}
}
