using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public string NextLevel;
	public static bool levelCompleted;
	private static GameObject[] breakables;

	void Start() {
		GameManager.levelCompleted = false;
		GameFader.StartScene();
	}

	void Update() {
		if (GameManager.levelCompleted) {
			GameFader.EndScene(NextLevel);
		}
	}

	public static void MakeBreakablesTrigger(bool isTrigger = true) {
		GameManager.breakables = GameObject.FindGameObjectsWithTag("Breakable");
		foreach (GameObject breakable in GameManager.breakables) {
			breakable.GetComponent<BoxCollider>().isTrigger = isTrigger;
		}
	}
}
