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
		
		GenerateLevel();
		
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
		
	
	public GameObject[] sectionPrefabs;
	List<GameObject> sections = new List<GameObject>();
	public void GenerateLevel() {
		states.Clear();
		foreach (var section in sections) {
			DestroyImmediate(section);
		}
		sections.Clear();
		
		var firstOffset = new Vector3(-50, 0, -50);
		for (var y = 0; y < 5; ++y) {
			for (var x = 0; x < 5; ++x) {
				// Skip the section where the portal is
				if (x != 2 || y != 4) {
					var sectionPrefab = sectionPrefabs[Random.Range(0, sectionPrefabs.Length)];
					
					var section = (GameObject)Instantiate(sectionPrefab, firstOffset + new Vector3(x * 25, 0, y * 25), Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
					sections.Add(section);
				}
			}
		}
	}
}
