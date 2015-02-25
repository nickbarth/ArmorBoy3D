using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;
	public float timer;

	void Update () {
		if (!Player.dead) {
			transform.position = new Vector3 (target.position.x, transform.position.y, transform.position.z);
			timer = 0f;
		} else {
			timer += Time.deltaTime;
			transform.position = Vector3.Lerp(transform.position, new Vector3(Player.lastCheckPoint.transform.position.x, transform.position.y, transform.position.z), timer / 2);
		}
	}
}
