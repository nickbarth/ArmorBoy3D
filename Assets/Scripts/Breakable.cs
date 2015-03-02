using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
	public GameObject pieces;

	public void Break () {
		Object explosion = Instantiate(pieces, transform.position, Quaternion.identity);
		Destroy(gameObject);
		Destroy(explosion, 1f);
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player" && Player.Attacking) {
			Break();
		}
	}
}