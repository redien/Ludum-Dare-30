using UnityEngine;
using System.Collections;

public class Interaction : MonoBehaviour {
	
	public Transform debugPoint;
	
	// Use this for initialization
	void Start () {
		
	}
	
	Interactive FindInteractiveIfExists(Transform child) {
		var interactive = child.GetComponent<Interactive>();
		if (interactive != null) {
			return interactive;
		} else {
			if (child.parent != null) {
				return FindInteractiveIfExists(child.parent);
			} else {
				return null;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
		RaycastHit hitInfo;

		if (Physics.Raycast(ray, out hitInfo)) {
			var hitGameObject = hitInfo.collider.gameObject;
			
			debugPoint.position = hitInfo.point;
			
			if (Input.GetMouseButtonDown(0)) {
				var interactive = FindInteractiveIfExists(hitGameObject.transform);
				if (interactive) {
					interactive.Interact();
				}
			}
		}
	}
}
