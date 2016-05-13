using UnityEngine;
using System.Collections;
using ParticlePlayground;
using Thinksquirrel.Fluvio;


public class particlesAtCollision : MonoBehaviour {

	public PlaygroundParticlesC part;
	public PlaygroundParticlesC part2;
//	public FluidParticleSystem part3;
//	private ParticleSystem party;
	private bool colliding;

	// Use this for initialization
	void Start () {
		part.emit = false;
//		party = part3.GetParticleSystem ();
	
	}


	void Update()
	{
		//colliding = false;
		part.emit = colliding;
		part2.emit = colliding;

//		party.enableEmission = colliding;
	//	colliding = false;
	}

	void OnCollisionStay(Collision coll)
	{
		if (coll.transform.tag == "canBleed") {
			Debug.Log ("Found");
			part.transform.position = coll.contacts [0].point;// + (transform.rotation.eulerAngles * 0.0001f);
//			part3.transform.position = coll.contacts [0].point;
			colliding = true;
		}
	}

	void OnCollisionExit(Collision coll)
	{
		colliding = false;
	}
}
