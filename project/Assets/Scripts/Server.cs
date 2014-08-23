using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {
	
	public string version = "version 1";
	
	public Settings settings;
	public GUIStyle serverNameStyle;
	
	[HideInInspector]
	public string serverName;
	
	class Player {
		public NetworkPlayer networkPlayer;
	}

	Player[] players = new Player[32];

	// Use this for initialization
	void Start () {
		settings = GameObject.Find("Global").GetComponent<Settings>();
		
		// Pause game when we are doing menu stuff...
		settings.Pause();
		
		// Start testing connection
		Network.TestConnection();
	}
	
	// Update is called once per frame
	void Update () {
	}

	enum NetworkingState {
		SelectRole,
		DeterminingConnection,
		WaitingForConnection,
		InputServerName,
		ConnectingToServer,
		Connected
	}
	
	NetworkingState networkState = NetworkingState.SelectRole;
	
	string[] randomAdjectives = {
		"proper", "speedy", "working", "sweet", "amazing", "cool", "special",
		"magical", "enchanting", "busy", "cute", "basic", "slow", "frantic",
		"odd", "even"
	};
	
	string[] randomNouns = {
		"cat", "lizard", "dog", "panda", "game", "pants", "shoe", "tree",
		"bee", "music", "car", "puppy", "craft", "saga", "king", "candy",
		"space"
	};

	string GenerateRandomName() {
		return randomAdjectives[Random.Range(0, randomAdjectives.Length - 1)] + " " + randomNouns[Random.Range(0, randomNouns.Length - 1)];
	}
	
	string serverToJoin;
	
	void OnGUI() {
		if (networkState == NetworkingState.SelectRole) {
			if (GUI.Button(new Rect(50, 50, 100, 75), "Host a game")) {
				networkState = NetworkingState.DeterminingConnection;
			}

			if (GUI.Button(new Rect(200, 50, 200, 75), "Join another game")) {
				networkState = NetworkingState.InputServerName;
				MasterServer.ClearHostList();
				MasterServer.RequestHostList("jesosk_LD30");
			}
		}

		if (networkState == NetworkingState.DeterminingConnection) {
			var connectionTestResult = Network.TestConnection();
			
			if (connectionTestResult != ConnectionTesterStatus.Undetermined) {
				Debug.Log(connectionTestResult.ToString());
				
				var useNat = !Network.HavePublicAddress();
				Network.InitializeServer(32, 25000, useNat);
				serverName = GenerateRandomName();
				MasterServer.RegisterHost("jesosk_LD30", serverName, version);
				networkState = NetworkingState.WaitingForConnection;
				settings.Resume();
			}
		}

					
		if (networkState == NetworkingState.InputServerName) {
			var hosts = MasterServer.PollHostList();
			
			if (hosts.Length > 0) {
				serverToJoin = GUI.TextField(new Rect(10, 50, 180, 75), serverToJoin);
				
				if (GUI.Button(new Rect(200, 50, 200, 75), "Join")) {
					HostData server = null;
					foreach (var host in hosts) {
						if (host.gameName == serverToJoin && host.comment == version) {
							server = host;
						}
					}
					
					if (server != null) {
						var error = Network.Connect(server);
						networkState = NetworkingState.ConnectingToServer;
						if (error != NetworkConnectionError.NoError) {
							Debug.Log("Connection error: " + error.ToString());
						}
					}
				}
			} else {
				GUI.Label(new Rect(10, 10, 300, 100), "Looking for games...", serverNameStyle);
			}
		}
		
		if (networkState == NetworkingState.WaitingForConnection || networkState == NetworkingState.Connected) {
			GUI.Label(new Rect(10, 10, 300, 100), serverName, serverNameStyle);
		}
	}

	int GetAvailablePlayerSlot() {
		for (var i = 0; i < players.Length; ++i) {
			if (players[i] == null) {
				return i;
			}
		}
		
		return -1;
	}
	
	int GetPlayerIdFromNetworkPlayer(NetworkPlayer networkPlayer) {
		for (var i = 0; i < players.Length; ++i) {
			if (players[i] != null && players[i].networkPlayer == networkPlayer) {
				return i;
			}
		}
		
		return -1;
	}
	
	void OnPlayerDisconnected(NetworkPlayer networkPlayer) {
        Debug.Log("Player " + networkPlayer.ToString() + " disconnected");
		
		var playerId = GetPlayerIdFromNetworkPlayer(networkPlayer);
		if (playerId != -1) {
			players[playerId] = null;
		}
    }

	void OnPlayerConnected(NetworkPlayer networkPlayer) {
        var playerId = GetAvailablePlayerSlot();
		
		Debug.Log("Player " + networkPlayer.ToString() + " connected");
		
		if (playerId != -1) {
			var player = new Player();
			player.networkPlayer = networkPlayer;
			players[playerId] = player;
			
			networkView.RPC("SetServerName", networkPlayer, serverName);
		} else {
			Debug.Log("Server full");
			Network.CloseConnection(networkPlayer, true);
		}
    }
	
	[RPC]
	void SetServerName(string name) {
		serverName = name;
	}
	
	void OnConnectedToServer() {
		settings.Resume();
		networkState = NetworkingState.Connected;
	}
}
