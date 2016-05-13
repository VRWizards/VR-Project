using UnityEngine;
using System.Collections;
using Leap;
using UnityEngine.UI;

//All the counting stuff was from RollABall in PlayerController.cs

public class moveProjectile : MonoBehaviour {

	private Rigidbody rig;
	public float force = 100000;
	private Quaternion rot;
	public HandModel hand;
	public Transform target;


	private int count;

	// Use this for initialization
	void Start () {

		target = GameObject.Find ("Target").transform;

	//	Quaternion lookAt = Quaternion.LookRotation(hand.GetPalmRotation() - transform.position);
		//rot = Quaternion.FromToRotation(Vector3.forward, ;
		rig = GetComponent<Rigidbody> ();
		//rig.AddForce (hand.GetPalmRotation().eulerAngles * force);
		rig.AddForce ((transform.position - target.transform.position) * force);
		//rig.AddForce ((hand.GetPalmDirection()) * force);



	}

	void OnTriggerEnter (Collider coll)
	{
		if (coll.tag == "Pick Up") {
			//coll.gameObject.SetActive(false);
			coll.GetComponent<EnemyHealth>().TakeDamage(10,coll.transform.position);

			//need to send info to player shooting that I have shot something

			PlayerController.count++;

			//Debug.Log ("count", coll);
			PlayerController.SetCountText();

		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//transform.position = Vector3.MoveTowards(transform.position, target.position, force);
	}


}
