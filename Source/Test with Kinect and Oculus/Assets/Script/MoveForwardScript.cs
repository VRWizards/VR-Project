using UnityEngine;
using System.Collections;

public class MoveForwardScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPosition = this.transform.position;
		newPosition.z += .1f;

		this.transform.position = newPosition;
	}
}
