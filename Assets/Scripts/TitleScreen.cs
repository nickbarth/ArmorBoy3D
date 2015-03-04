using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
  public float timer;

  // Config
  public Vector3 StartPosition;
  public Vector3 EndPosition;
  public Vector3 TargetPosition;

  public GameObject Title;
  public Vector3 TitleStartPosition;
  public Vector3 TitleEndPosition;
  public Vector3 TitleTargetPosition;

  public GameObject Text;

  void Awake() {
    GameFader.StartScene();
    TargetPosition = EndPosition;
    TitleTargetPosition = TitleEndPosition;
  }

  void Start() {
    #if UNITY_IOS || UNITY_ANDROID
    Text.GetComponent<Text>().text = "Press Screen to Start";
    #endif
  }

  void Update() {
    timer += Time.deltaTime;
    transform.position = Vector3.Lerp(transform.position, TargetPosition, timer / 10);
    Title.transform.position = Vector3.Lerp(Title.transform.position, TitleTargetPosition, timer / 10);

    if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)) {
      GameFader.EndScene("Level1");
    }

    if (transform.position == EndPosition) {
      timer = 0;
      TargetPosition = StartPosition;
      TitleTargetPosition = TitleStartPosition;
    } else if (transform.position == StartPosition) {
      timer = 0;
      TargetPosition = EndPosition;
      TitleTargetPosition = TitleEndPosition;
    }
  }
}
