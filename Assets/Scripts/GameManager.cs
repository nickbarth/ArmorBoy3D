using UnityEngine;

public class GameManager : MonoBehaviour {
  public static GameObject ArmorBoy { get; set; }
  public static Player Player { get; set; }
  public static bool LevelCompleted { get; set; }

  private static GameObject[] breakables;

  // Config
  public string NextLevel;

  void Awake() {
    #if UNITY_IOS || UNITY_ANDROID
    GameObject.Find("LeftButton").SetActive(true);
    GameObject.Find("RightButton").SetActive(true);
    GameObject.Find("JumpButton").SetActive(true);
    GameObject.Find("AttackButton").SetActive(true);
    #else
    GameObject.Find("LeftButton").SetActive(false);
    GameObject.Find("RightButton").SetActive(false);
    GameObject.Find("JumpButton").SetActive(false);
    GameObject.Find("AttackButton").SetActive(false);
    #endif

    GameManager.LevelCompleted = false;
    GameFader.StartScene();

    GameManager.ArmorBoy = GameObject.Find("ArmorBoy");
    GameManager.Player = ArmorBoy.GetComponent<Player>();
  }

  void Update() {
    if (GameManager.LevelCompleted) {
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
