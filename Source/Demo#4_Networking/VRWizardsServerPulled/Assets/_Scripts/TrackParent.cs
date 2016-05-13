using UnityEngine;
using System.Collections;

public class TrackParent : MonoBehaviour {

	private CapsuleCollider coll;
	public GameObject toBleed;

	// Use this for initialization
	void Start () {
		coll = GetComponent<CapsuleCollider> ();
	}
	
	// Update is called once per frame
	void Update () {
		//coll.radius = toBleed.GetComponent<CapsuleCollider> ().radius;
	//	coll.height = toBleed.GetComponent<CapsuleCollider> ().height*20;
		transform.position = SelfHarm.passPos;
		transform.rotation = SelfHarm.passRot;
	}
}
