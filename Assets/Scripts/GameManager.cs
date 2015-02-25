using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public string NextLevel;
	public static bool levelCompleted;
	private static GameObject[] breakables;

	void Start() {
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		GameObject.Find("LeftButton").SetActive(false);
		GameObject.Find("RightButton").SetActive(false);
		GameObject.Find("JumpButton").SetActive(false);
		GameObject.Find("AttackButton").SetActive(false);
		#endif

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
