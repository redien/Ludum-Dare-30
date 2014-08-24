using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour {
	
	public GameObject player;
	public Vector3 initialPlayerPosition;
	public Fader fader;

	public List<State> states = new List<State>();
	public Portal portal;
	
	PhotonView photonView;
	
	// Use this for initialization
	void Awake () {
		Application.runInBackground = true;
		photonView = GetComponent<PhotonView>();
		
		initialPlayerPosition = player.transform.position;
	}

	// Update is called once per frame
	void Update () {
		var shouldEnablePortal = true;
		foreach (var state in states) {
			if (!state.GetState()) {
				shouldEnablePortal = false;
				break;
			}
		}
		portal.SetState(shouldEnablePortal);
	}
	
	public void LoadNextLevel() {
		StartCoroutine(NewLevelFade());
		photonView.RPC("SomeoneCompletedLevel", PhotonTargets.Others);
	}
	
	[RPC]
	void SomeoneCompletedLevel() {
		StartCoroutine(NewLevelFade());
	}
	
	IEnumerator NewLevelFade() {
		Pause();
		yield return new WaitForSeconds(2.0f);

		player.transform.position = initialPlayerPosition;
		player.transform.localRotation = Quaternion.identity;
		
		yield return new WaitForSeconds(2.0f);
		Resume();
	}

	public void Pause() {
		Screen.lockCursor = false;
		fader.fadeIn = false;
	}

	public void Resume() {
		Screen.lockCursor = true;
		fader.fadeIn = true;
	}
}
