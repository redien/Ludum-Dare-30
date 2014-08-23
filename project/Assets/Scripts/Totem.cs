using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour, InteractiveObject {
	
	public GameObject disabledObject;
	public GameObject enabledObject;
	public float interactDistance = 10.0f;
	
	PhotonView photonView;

	void Awake () {
		var interactive = GetComponent<Interactive>();
		interactive.interactiveObject = this;
		
		photonView = GetComponent<PhotonView>();
	}
	
	bool state = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Interact(Transform interacter) {
		if (CanInteract(interacter)) {
			audio.Play();
			SetState(!state);
			photonView.RPC("SetRemoteState", PhotonTargets.Others, (state ? 1 : 0));
		}
	}
	
	public bool CanInteract(Transform interacter) {
		return Vector3.Distance(interacter.position, transform.position) < interactDistance;
	}
	
	[RPC]
	void SetRemoteState(int state) {
		audio.Play();
		SetState(state != 0);
	}

	void SetState(bool state) {
		this.state = state;
		
		if (state) {
			disabledObject.SetActive(false);
			enabledObject.SetActive(true);
		} else {
			disabledObject.SetActive(true);
			enabledObject.SetActive(false);
		}
	}

	void OnPhotonPlayerConnected(PhotonPlayer player) {
		if (PhotonNetwork.isMasterClient) {
			photonView.RPC("SetRemoteState", player, (state ? 1 : 0));
		}
	}
}
