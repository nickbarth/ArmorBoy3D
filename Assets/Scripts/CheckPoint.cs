using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {
	public GameObject[] enemies;

	public void Respawn () {
		foreach (GameObject enemy in enemies) {
			enemy.gameObject.GetComponent<Enemy>().Respawn();
		}
	}
}
