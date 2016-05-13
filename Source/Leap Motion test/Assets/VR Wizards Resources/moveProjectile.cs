using UnityEngine;
using System.Collections;

public class moveProjectile : MonoBehaviour {

	private Rigidbody rig;
	public float force = 100 ;
	public float lifetime = 2;
	public ParticleSystem part;

	// Use this for initialization
	void Start () {
		rig = GetComponent<Rigidbody> ();
		Vector3 fwd = transform.TransformDirection (Vector3.forward);
		//Vector3 fwd = new Vector3(0,0,force);
		rig.AddForce (fwd * force);
		Invoke ("Suicide", lifetime);
	}

	void Suicide()
	{
		Destroy (this.gameObject);
	}

	void OnCollisionEnter(Collision coll)
	{
		if (coll.transform.tag == "canBleed") {
			//rig.useGravity = true;
			part.Emit (3);
		}
	}
	

}
