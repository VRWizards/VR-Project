using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rotation2Script : MonoBehaviour {
	
	public float rotationPerSecond= 0.1f;
	private List<GameObject> children;
	private List<string> parentNames = new List<string>(){"links.001","rechts.001","oben.001","unten.001"};
	// Use this for initialization
	void Start () {
		children = Utilities.GetChildren (gameObject);
	}
	
	// Update is called once per frame
	void Update () {


		foreach (GameObject child in children) {
			if(parentNames == null || child == null || parentNames.Contains(child.name)) continue;
			if (child.GetComponent<Rigidbody>() != null && child.activeSelf && child.GetComponent<Rigidbody>().useGravity) continue;
			child.transform.RotateAround (transform.position,child.transform.up, Time.deltaTime * rotationPerSecond * 360.0f);
		}

	}
}
