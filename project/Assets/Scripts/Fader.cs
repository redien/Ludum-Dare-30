using UnityEngine;
using System.Collections;

public class Fader : MonoBehaviour {
	
	public GameObject fadeObject;
	
	Renderer renderer;
	float alpha = 1.0f;
	
	public bool fadeIn = false;
	
	void Awake () {
		renderer = fadeObject.GetComponent<Renderer>();
		var material = renderer.sharedMaterial;
		material.color = new Color(0, 0, 0, alpha);
		renderer.sharedMaterial = material;
	}

	void Update () {
		var material = renderer.sharedMaterial;
		material.color = new Color(0, 0, 0, alpha);
		renderer.sharedMaterial = material;
		
		if (fadeIn) {
			alpha -= Time.deltaTime * 0.5f;
		} else {
			alpha += Time.deltaTime * 0.5f;
		}
		
		alpha = Mathf.Clamp(alpha, 0.0f, 1.0f);
	}
}
