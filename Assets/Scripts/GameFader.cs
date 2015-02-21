using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFader : MonoBehaviour
{
	public string NextLevel;

	private static RectTransform rt;
	private static Image image;
	private static bool sceneStart = false;
	private static bool sceneEnd = false;
	private static float timer = 0f;
	private static Color fade = Color.black;
	private static string goToLevel;
	private static GameObject fader;

	void Awake ()
	{
		fader = gameObject;
		rt = transform.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(Screen.width, Screen.height);
		image = transform.GetComponent<Image>();
		image.color = fade;
	}

	void Start () {
		goToLevel = NextLevel;
		StartScene();
	}

	void Update ()
	{
		if (sceneStart) {
			FadeTo(Color.clear);
			
			if (image.color.a <= 0.05f) { 
				timer = 0f;
				sceneStart = false;
				image.color = Color.clear;
				fader.SetActive(false);
			}
		}

		if (sceneEnd) {
			FadeTo(fade);
			
			if (image.color.a >= 0.95f) { 
				timer = 0f;
				sceneEnd = false;
				image.color = fade;
				Application.LoadLevel(goToLevel);
			}
		}
	}

	private static void FadeTo(Color color)
	{
		timer += Time.deltaTime;
		image.color = Color.Lerp(image.color, color, timer / 5);
	}

	public static void StartScene ()
	{
		sceneStart = true;
		sceneEnd = false;
	}

	public static void EndScene ()
	{
		sceneStart = false;
		sceneEnd = true;
		fader.SetActive(true);
	}
}
