using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	
	public GameObject enabledPortal;
	public GameObject disabledPortal;
	
	Settings settings;
	
	void Awake() {
		settings = GameObject.Find("Global").GetComponent<Settings>();
	}
	
	bool state = false;
	public void SetState(bool state) {
		if (state && !this.state) {
			disabledPortal.SetActive(false);
			enabledPortal.SetActive(true);
		}
		
		if (!state && this.state) {
			disabledPortal.SetActive(true);
			enabledPortal.SetActive(false);
		}

		this.state = state;
	}
	
	void OnTriggerEnter (Collider other) {
		if (state && other.tag == "Player") {
			settings.LoadNextLevel();
		}
	}
}
