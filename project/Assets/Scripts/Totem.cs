using UnityEngine;
using System.Collections;

public class Totem : MonoBehaviour, InteractiveObject {
	
	public GameObject disabledObject;
	public GameObject enabledObject;
	
	void Awake () {
		var interactive = GetComponent<Interactive>();
		interactive.interactiveObject = this;
	}
	
	bool state = false;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Interact() {
		audio.Play();
		SetState(!state);
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
}
