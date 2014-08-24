using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour, InteractiveObject, State {
	
	public GameObject disabledObject;
	public GameObject enabledObject;
	public float interactDistance = 10.0f;
	
	PhotonView photonView;

	void Awake () {
		var interactive = GetComponent<Interactive>();
		interactive.interactiveObject = this;
		
		photonView = GetComponent<PhotonView>();
		
		var settings = GameObject.Find("Global").GetComponent<Settings>();
		settings.states.Add(this);
		photonView.viewID = settings.states.Count;
	}
	
	// Reference to the actual state object
	public WorldState.State state;

	public void Interact(Transform interacter) {
		if (CanInteract(interacter)) {
			audio.Play();
			SetState(!state.enabled);
			photonView.RPC("SetRemoteState", PhotonTargets.Others, (state.enabled ? 1 : 0));
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
	
	public bool GetState() {
		return state.enabled;
	}

	public void SetState(bool state) {
		this.state.enabled = state;
		
		if (state) {
			disabledObject.SetActive(false);
			enabledObject.SetActive(true);
		} else {
			disabledObject.SetActive(true);
			enabledObject.SetActive(false);
		}
	}
}
