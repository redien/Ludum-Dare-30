using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour, InteractiveObject {
	
	public GameObject disabledObject;
	public GameObject enabledObject;
	public GameObject delayedObject;
	
	public GameObject linkedStatue;
	public GameObject defaultStatue;
	
	public float interactDistance = 10.0f;
	
	public AudioSource ticking;
	
	void Awake () {
		var interactive = GetComponent<Interactive>();
		interactive.interactiveObject = this;
	}
	
	StateCollection.State stateSettings;
	
	void Start () {
		// Initialize from spawner
		var stateSpawner = transform.parent.GetComponent<StateSpawner>();
		stateCollection = stateSpawner.stateCollection;
		stateId = stateSpawner.stateId;
		
		// Customize based on state settings
		stateSettings = stateCollection.GetStateSettings(stateId);
		if (stateSettings.disableAfter > 0.0f) {
			enabledObject.SetActive(false);
			enabledObject = delayedObject;
			enabledObject.SetActive(true);
		}

		if (stateSettings.statesToEnableOnEnable != null || stateSettings.statesToDisableOnEnable != null) {
			linkedStatue.SetActive(true);
			defaultStatue.SetActive(false);
		}

		UpdateState();
	}
	
	// Reference to the actual state object
	public StateCollection stateCollection;
	public int stateId;

	public void Interact(Transform interacter) {
		if (CanInteract(interacter)) {
			audio.Play();
			
			if (stateSettings.disableAfter > 0.0f) {
				if (!GetState()) {
					ticking.Play();
				} else {
					ticking.Stop();
				}
			}
			
			SetState(!GetState());
		}
	}
	
	public bool CanInteract(Transform interacter) {
		return Vector3.Distance(interacter.position, transform.position) < interactDistance;
	}

	public bool GetState() {
		return stateCollection.GetState(stateId);
	}
	
	public void SetState(bool state) {
		this.stateCollection.SetState(stateId, state);
		UpdateState();
	}
	
	public void UpdateState() {
		if (this.stateCollection.GetState(stateId)) {
			disabledObject.SetActive(false);
			enabledObject.SetActive(true);
		} else {
			disabledObject.SetActive(true);
			enabledObject.SetActive(false);
		}
	}
	
	void Update() {
		if (this.stateCollection.GetState(stateId) != enabledObject.activeSelf) {
			// The totem is not insync with the state
			audio.Play();
			UpdateState();
		}
	}
}
