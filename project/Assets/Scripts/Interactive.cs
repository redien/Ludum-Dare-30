using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour {
	public InteractiveObject interactiveObject;
	
	public void Interact() {
		interactiveObject.Interact();
	}
}
