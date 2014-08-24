using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour {
	
	public GameObject player;
	public Vector3 initialPlayerPosition;
	public Fader fader;

	public List<State> states = new List<State>();
	public Portal portal;
	public WorldState worldState;
	
	PhotonView photonView;
	
	public int level = 0;
	
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
		worldState = new WorldState();
		worldState.Generate(level);

		StartCoroutine(NewLevelFade(worldState));
		
		/* Serialize states to send over RPC */
		string serializedWorldState = WorldState.Serialize(worldState);
		
		photonView.RPC("SomeoneCompletedLevel", PhotonTargets.Others, serializedWorldState);
	}
	
	[RPC]
	void SomeoneCompletedLevel(string serializedWorldState) {
		/* Deserialize states we got over RPC */
		worldState = WorldState.Deserialize(serializedWorldState);
		StartCoroutine(NewLevelFade(worldState));
	}

	IEnumerator NewLevelFade(WorldState worldState) {
		Pause();
		yield return new WaitForSeconds(2.0f);
		
		GenerateLevel(worldState);
		level += 1;
		
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
	
	public GameObject[] stateSectionPrefabs;
	public GameObject[] staticSectionPrefabs;
	List<GameObject> sections = new List<GameObject>();

	public void GenerateLevel(WorldState worldState) {
		states.Clear();
		foreach (var section in sections) {
			DestroyImmediate(section);
		}
		sections.Clear();
		
		var statesSpawned = 0;
		
		var firstOffset = new Vector3(-50, 0, -50);
		for (var y = 0; y < 5; ++y) {
			for (var x = 0; x < 5; ++x) {
				// Skip the section where the portal is
				if (x != 2 || y != 4) {
					GameObject sectionPrefab;
					bool spawnedState = false;
					Vector3 position = firstOffset + new Vector3(x * 25, 0, y * 25);
					Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
					
					if (statesSpawned < worldState.Count) {
						sectionPrefab = stateSectionPrefabs[worldState.GetPrefabOf(statesSpawned)];
						spawnedState = true;
					} else {
						sectionPrefab = staticSectionPrefabs[Random.Range(0, staticSectionPrefabs.Length)];
					}

					var section = (GameObject)Instantiate(sectionPrefab, position, rotation);
					
					// Initialize states
					if (spawnedState) {
						var totem = section.GetComponentInChildren<Totem>();
						if (totem) {
							totem.state = worldState.states[statesSpawned];
							totem.SetState(totem.GetState());
						}
						statesSpawned += 1;
					}
					
					sections.Add(section);
				}
			}
		}
	}
}
