using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class particlesAtCollision : MonoBehaviour {

	public PlaygroundParticlesC part;
	private bool colliding;

	// Use this for initialization
	void Start () {
		part.emit = false;
	
	}


	void Update()
	{
		//colliding = false;
		part.emit = colliding;
	//	colliding = false;
	}

	void OnCollisionStay(Collision coll)
	{
		if (coll.transform.tag == "canBleed") {
			Debug.Log ("Found");
			part.transform.position = coll.contacts [0].point + (transform.rotation.eulerAngles * 0.0001f);
			colliding = true;
		}
	}

	void OnCollisionExit(Collision coll)
	{
		colliding = false;
	}
}
