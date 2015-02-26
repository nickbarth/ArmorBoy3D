using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public bool dead;
	public GameObject DeathParticles;
	public RaycastHit hit;
	public bool forwardHit;
	public bool backwardHit;
	public bool goingForward;
	public float moveSpeed;

	private bool hasGravity;
	private Vector3 pos;
	private Quaternion rot;

	public GameObject WaitObject;
	public Color myColor;

	public bool fading;
	public float alpha;

	private SpriteRenderer sprite; 

	void Start() {
		fading = false;
		sprite = gameObject.GetComponent<SpriteRenderer>();
		alpha = 1f;
		hasGravity = rigidbody.useGravity;
		pos = transform.position;
		rot = transform.rotation;
		goingForward = true;
		if (moveSpeed == 0) {
			moveSpeed = 0.03f;
		}
	}

	void Update() {
		if (fading) {
			alpha -= Time.deltaTime * 2;
			sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, alpha);
			Respawn();
		}

		if (!fading && alpha < 1f) {
			alpha += Time.deltaTime;
			sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, alpha);
		}

		if (dead) return;
		if (WaitObject != null && WaitObject.activeSelf) return;

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
		if (collider.gameObject.tag == "BadTouch") {
			Kill();
		}

		if (collider.gameObject.tag == "Player") {
			Kill();
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "BadTouch") {
			Kill();
		}

		if (collision.gameObject.tag == "Player") {
			Kill();
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

	IEnumerator Fade() {
		Debug.Log ("Fading out");
		yield return new WaitForSeconds(0.5f);
	}
	
	public void Respawn() {
		if (alpha > 0f) {
			fading = true;
			return;
		}

		goingForward = true;
		transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
		transform.rotation = rot;
		transform.position = pos;
		gameObject.renderer.enabled = true;
		collider.enabled = true;
		goingForward = true;
		if (hasGravity) {
			rigidbody.useGravity = true;
		}
		sprite.color = new Color(sprite.material.color.r, sprite.material.color.g, sprite.material.color.b, 1f);
		dead = false;
		fading = false;
	}
}
