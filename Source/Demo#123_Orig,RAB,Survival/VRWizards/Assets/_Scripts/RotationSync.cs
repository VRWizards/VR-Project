using UnityEngine;
using System.Collections;

public class RotationSync : MonoBehaviour {

	public GameObject rotator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {

		transform.localRotation = rotator.transform.localRotation;
		//transform.LookAt (rotator.transform);
	
	}
}
