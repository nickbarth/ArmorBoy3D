using UnityEngine;
using System.Collections;

public class GameMusic : MonoBehaviour {
	public AudioClip attackSound;
	public AudioClip injureSound;
	public AudioClip crashSound;
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public static void PlayAttack() {
		var sounds = GameObject.Find("GameMusic") as GameObject;
		AudioSource.PlayClipAtPoint(sounds.GetComponent<GameMusic>().attackSound, sounds.transform.position);
	}
	
	public static void PlayInjure() {
		var sounds = GameObject.Find("GameMusic") as GameObject;
		AudioSource.PlayClipAtPoint(sounds.GetComponent<GameMusic>().injureSound, sounds.transform.position);
	}
	
	public static void PlayCrash() {
		var sounds = GameObject.Find("GameMusic") as GameObject;
		AudioSource.PlayClipAtPoint(sounds.GetComponent<GameMusic>().crashSound, sounds.transform.position);
	}
}
