using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {
	
	public GameObject player;
	
	// Use this for initialization
	void Awake () {
		Application.runInBackground = true;
	}

	// Update is called once per frame
	void Update () {
	
	}
	
	public void Pause() {
		Time.timeScale = 0.0f;
		player.SetActive(false);
		Screen.lockCursor = false;
	}

	public void Resume() {
		Time.timeScale = 1.0f;
		player.SetActive(true);
		Screen.lockCursor = true;
	}
}
