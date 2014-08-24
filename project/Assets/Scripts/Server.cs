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
		NewServerError,
		InputServerName,
		ConnectingToServer,
		ConnectionError,
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
		/*
		if (PhotonNetwork.connectionStateDetailed.ToString() != lastText) {
			text += PhotonNetwork.connectionStateDetailed.ToString() + ", ";
			lastText = PhotonNetwork.connectionStateDetailed.ToString();
		}

		GUILayout.Label(text);
		*/

		if (!PhotonNetwork.connected) {
	    } else {
			PhotonNetwork.GetRoomList();
			
			GUILayout.BeginArea(new Rect (Screen.width/2 - 200, Screen.height/2 - 200, 400, 400));
			
			if (networkState == NetworkingState.SelectRole) {
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Host a game", GUILayout.MinWidth(100), GUILayout.MinHeight(50))) {
					serverName = GenerateRandomName();
					PhotonNetwork.CreateRoom(serverName);
				}
	
				if (GUILayout.Button("Join another game", GUILayout.MinWidth(150), GUILayout.MinHeight(50))) {
					networkState = NetworkingState.InputServerName;
				}
				GUILayout.EndHorizontal();
			}

			if (networkState == NetworkingState.InputServerName) {
				GUILayout.BeginVertical();
				GUI.SetNextControlName("ServerNameBox");
				serverToJoin = GUILayout.TextField(serverToJoin, serverNameStyle, GUILayout.MinWidth(300), GUILayout.MinHeight(50));
				GUI.FocusControl("ServerNameBox");

				GUILayout.BeginHorizontal();
				if (serverToJoin.Length > 0) {
					if (GUILayout.Button("Join", GUILayout.MinWidth(100), GUILayout.MinHeight(50))) {
						PhotonNetwork.JoinRoom(serverToJoin);
						serverName = serverToJoin;
						networkState = NetworkingState.ConnectingToServer;
					}
				}

				if (GUILayout.Button("Back", GUILayout.MinWidth(100), GUILayout.MinHeight(50))) {
					networkState = NetworkingState.SelectRole;
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

			if (networkState == NetworkingState.ConnectingToServer) {
				GUILayout.Label("Connecting to game...", serverNameStyle);
			}

			if (networkState == NetworkingState.ConnectionError || networkState == NetworkingState.NewServerError) {
				GUILayout.Label("Error: " + errorMessage, serverNameStyle);
				GUILayout.Space(10);
				if (GUILayout.Button("Try again", GUILayout.MinWidth(100), GUILayout.MinHeight(50))) {
					if (networkState == NetworkingState.NewServerError) {
						networkState = NetworkingState.SelectRole;
					} else {
						networkState = NetworkingState.InputServerName;
					}
				}
			}

			GUILayout.EndArea ();
			
			if (networkState == NetworkingState.Connected) {
				GUILayout.Label("Other players can use this name to connect:");
				GUILayout.BeginHorizontal();
				GUILayout.Space(30);
				GUILayout.Label(serverName, serverNameStyle);
				GUILayout.EndHorizontal();
			}
		}
	}

	string errorMessage;
	
	void OnPhotonJoinRoomFailed() {
		errorMessage = "Something went wrong...";
		networkState = NetworkingState.ConnectionError;
	}
	
	void OnPhotonCreateRoomFailed() {
		errorMessage = "Something went wrong, please try again.";
		networkState = NetworkingState.NewServerError;
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
