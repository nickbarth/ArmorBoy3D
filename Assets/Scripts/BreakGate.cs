using UnityEngine;
using System.Collections;

public class BreakGate : MonoBehaviour {
	public GameObject WoodExplode;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
		Object particles = Instantiate(WoodExplode, transform.position, Quaternion.identity);
		Destroy(this.gameObject);
		Destroy(particles, 1f);
	}
}
