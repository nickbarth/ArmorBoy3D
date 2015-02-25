using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {
	public GameObject[] enemies;

	public void Respawn () {
		Debug.Log ("called");
		foreach (GameObject enemy in enemies) {
			enemy.gameObject.GetComponent<Enemy>().Respawn();
			Debug.Log ("respawned");
		}
	}
}
