using UnityEngine;
using System.Collections;

public class StateSpawner : MonoBehaviour {
	
	public GameObject statePrefab;
	
	GameObject instance;
	
	[HideInInspector]
	public StateCollection stateCollection;
	[HideInInspector]
	public int stateId;
	
	void Awake () {
		instance = (GameObject)Instantiate(statePrefab);
		instance.transform.parent = transform;
		instance.transform.localPosition = Vector3.zero;
		instance.transform.localRotation = Quaternion.identity;
	}
}
