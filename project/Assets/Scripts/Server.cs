using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour {
	
	public string version = "version 1";
	
	public Settings settings;
	public GUIStyle serverNameStyle;
	
	[HideInInspector]
	public string serverName;

	PhotonView photonView;
	
	void Awake () {
		photonView = GetComponent<PhotonView>();
	}
	
	// Use this for initialization
	void Start () {
		settings = GameObject.Find("Global").GetComponent<Settings>();
		
		// Pause game when we are doing menu stuff...
		settings.Pause();

		// Start testing connection
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.ConnectUsingSettings("0.1");
	}
	
	// Update is called once per frame
	void Update () {
	}

	enum NetworkingState {
		SelectRole,
		WaitingForMaster,
		InputServerName,
		ConnectingToServer,
		Connected
	}
	
	NetworkingState networkState = NetworkingState.SelectRole;
	
	string[] randomAdjectives = {
		"proper", "speedy", "working", "sweet", "amazing", "cool", "special",
		"magical", "enchanting", "busy", "cute", "basic", "slow", "frantic",
		"odd", "even", "personal", "light", "heavy", "strong", "basic", "cold",
		"warm", "super", "tasty", "personal", "crazy", "tiny", "tall", "small",
		"shady"
	};

	string[] randomNouns = {
		"cat", "lizard", "dog", "panda", "game", "pants", "shoe", "tree",
		"bee", "music", "car", "puppy", "craft", "saga", "king", "candy",
		"space", "potato", "book", "paper", "beam", "castle", "rock", "box",
		"level", "bar", "cookie", "bread", "soup", "pizza", "milk", "potion",
		"sword", "hammer", "cup"
	};

	string GenerateRandomName() {
		return randomAdjectives[Random.Range(0, randomAdjectives.Length)] + " " + randomNouns[Random.Range(0, randomNouns.Length)];
	}
	
	string serverToJoin = "";
	string text = "";
	string lastText = "";
	
	void OnGUI() {
		if (PhotonNetwork.connectionStateDetailed.ToString() != lastText) {
			text += PhotonNetwork.connectionStateDetailed.ToString() + ", ";
			lastText = PhotonNetwork.connectionStateDetailed.ToString();
		}

		GUILayout.Label(text);
		
		if (!PhotonNetwork.connected) {
	    } else {
			PhotonNetwork.GetRoomList();
			
			if (networkState == NetworkingState.SelectRole) {
				if (GUI.Button(new Rect(50, 50, 100, 75), "Host a game")) {
					serverName = GenerateRandomName();
					PhotonNetwork.CreateRoom(serverName);
				}
	
				if (GUI.Button(new Rect(200, 50, 200, 75), "Join another game")) {
					networkState = NetworkingState.InputServerName;
				}
			}

			if (networkState == NetworkingState.InputServerName) {
				serverToJoin = GUI.TextField(new Rect(10, 50, 180, 75), serverToJoin);
				
				if (GUI.Button(new Rect(200, 50, 200, 75), "Join")) {
					PhotonNetwork.JoinRoom(serverToJoin);
					serverName = serverToJoin;
					networkState = NetworkingState.ConnectingToServer;
				}
			}
			
			if (networkState == NetworkingState.Connected) {
				GUI.Label(new Rect(10, 30, 300, 100), serverName, serverNameStyle);
			}
		}
	}

	void OnPhotonPlayerConnected(PhotonPlayer player) {
		if (PhotonNetwork.isMasterClient) {
			string stateCollectionSerialized = StateCollection.Serialize(settings.stateCollection);
			photonView.RPC("RecieveWorldState", player, stateCollectionSerialized);
		}
	}
	
	[RPC]
	void RecieveWorldState(string stateCollectionSerialized) {
		settings.stateCollection = StateCollection.Deserialize(stateCollectionSerialized);
		settings.GenerateLevel();
		settings.Resume();
		networkState = NetworkingState.Connected;
	}
	
	void OnJoinedRoom() {
		if (PhotonNetwork.isMasterClient) {
			settings.GenerateState();
			settings.GenerateLevel();
			settings.Resume();
			networkState = NetworkingState.Connected;
		}
	}
}
