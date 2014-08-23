using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour {
	public InteractiveObject interactiveObject;
	
	public void Interact(Transform interacter) {
		interactiveObject.Interact(interacter);
	}
	
	public bool CanInteract(Transform interacter) {
		return interactiveObject.CanInteract(interacter);
	}
}
