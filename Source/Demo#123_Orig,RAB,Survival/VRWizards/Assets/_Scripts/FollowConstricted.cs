using UnityEngine;
using System.Collections;

public class FollowConstricted : MonoBehaviour {

	public Transform target;
	public float offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 goal = new Vector3 (target.position.x + offset, transform.position.y, target.position.z);
		transform.position = goal;
	
	}
}
