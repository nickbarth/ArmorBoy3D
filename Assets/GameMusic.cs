using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour {
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
}
