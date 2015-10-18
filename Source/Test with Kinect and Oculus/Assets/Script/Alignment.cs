using UnityEngine;
using System.Collections;

public class Alignment : MonoBehaviour {
	
	private GameObject cameraRight;
	private GameObject cameraLeft;

	// Use this for initialization
	void Start () {
		cameraRight = GameObject.Find("CameraRight");
		cameraLeft = GameObject.Find("CameraLeft");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cameraPosition = (cameraRight.transform.position + cameraLeft.transform.position) / 2;
		transform.LookAt(cameraPosition, Vector3.up);
	}
}
