using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {
	
	public Transform playerPrefab;
	public Settings settings;
	
	class Player {
		public NetworkPlayer networkPlayer;
	}

	Player[] players = new Player[32];

	// Use this for initialization
	void Start () {
		settings = GameObject.Find("Global").GetComponent<Settings>();
		
		// Pause game when we are doing menu stuff...
		settings.Pause();
	}

	// Update is called once per frame
	void Update () {
	}
	
	enum NetworkingState {
		SelectRole,
		WaitingForConnection,
		ConnectingToServer
	}
	
	NetworkingState networkState = NetworkingState.SelectRole;

	void OnGUI() {
		if (networkState == NetworkingState.SelectRole) {
			if (GUI.Button(new Rect(50, 50, 100, 75), "Server")) {
				var useNat = !Network.HavePublicAddress();
				Network.InitializeServer(32, 25000, useNat);
				networkState = NetworkingState.WaitingForConnection;
				settings.Resume();
			}

			if (GUI.Button(new Rect(200, 50, 100, 75), "Client")) {
				var error = Network.Connect("127.0.0.1", 25000);
				networkState = NetworkingState.ConnectingToServer;
				if (error != NetworkConnectionError.NoError) {
					Debug.Log("Connection error: " + error.ToString());
				}
			}
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
		} else {
			Debug.Log("Server full");
			Network.CloseConnection(networkPlayer, true);
		}
    }
	
	void OnConnectedToServer() {
		settings.Resume();
	}
}
