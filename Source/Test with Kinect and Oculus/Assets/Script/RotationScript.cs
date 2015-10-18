using UnityEngine;
using System.Collections;

public class RotationScript : MonoBehaviour {

	public float rotationPerSecond= 0.1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3(0,1,0), Time.deltaTime * rotationPerSecond * 360.0f);
	}
}
