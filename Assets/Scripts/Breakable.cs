using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {
	public GameObject WoodExplode;
	
	public void Break () {
		Object particles = Instantiate(WoodExplode, transform.position, Quaternion.identity);
		Destroy(gameObject);
		Destroy(particles, 1f);
	}
}