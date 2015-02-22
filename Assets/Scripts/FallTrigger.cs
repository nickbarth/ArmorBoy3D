using UnityEngine;
using System.Collections;

public class FallTrigger : MonoBehaviour {
	public GameObject TriggerObject;

	void OnTriggerEnter(Collider collider) {
		if (collider.gameObject.tag == "Player") {
			Fallable faller = TriggerObject.gameObject.GetComponent<Fallable>();
			if (!faller.falling) {
				faller.Fall();
			}
		}
	}
}
