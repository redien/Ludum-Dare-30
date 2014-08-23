using UnityEngine;
using System.Collections;

public class Orbiting : MonoBehaviour {
	
	public Vector3 rotationAmount = Vector3.zero;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion localRotation = transform.localRotation;
		Vector3 rotation = localRotation.eulerAngles;
		rotation += rotationAmount * Time.deltaTime;
		localRotation.eulerAngles = rotation;
		transform.localRotation = localRotation;
	}
}
