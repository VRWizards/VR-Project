using UnityEngine;
using System.Collections;
using Leap;

public class Shoot : MonoBehaviour {

	public GameObject leapMotionOVRController = null;
	public HandController handController = null;
	public GameObject ammo;
	public float offset = -0.0632f;
	public float delay = 1;
	private bool canFire;
	public Transform target;




	// Use this for initialization
	void Start () {
		//target = GameObject.Find ("Target").transform;
		canFire = true;
		//target = transform.FindChild("Target");

	
	}
	
	// Update is called once per frame
	void Update () {

		HandModel[] hands = handController.GetAllGraphicsHands();
		if (hands.Length > 0) 
		{
			foreach (HandModel hand in hands)
			{

				//Debug.Log (hand.GetPalmRotation().eulerAngles.x);
				if (hand.GetPalmRotation().eulerAngles.x > 260 && hand.GetPalmRotation().eulerAngles.x < 290 && canFire)
				//if (Mathf.Abs(hand.GetPalmRotation().eulerAngles.x - hand.GetPalmDirection

					//Debug.Log (canFire );
					//Instantiate (ammo, hand.transform.position + hand.GetPalmNormal() * offset , hand.GetPalmRotation());

					StartCoroutine( Fire(hand));
					//canFire = false;
					//Timer();

			}
		}
	
	}

	IEnumerator Fire(HandModel currentHand)
	{
		canFire = false;
		//Debug.Log (canFire);

		GameObject justFired = Instantiate (ammo, currentHand.transform.position + currentHand.GetPalmNormal() * offset , currentHand.GetPalmRotation()) as GameObject;
		justFired.GetComponent<moveProjectile> ().target = target;
		//justFired.GetComponent<moveProjectile> ().force = Quaternion.LookRotation( handController.transform.forward, Vector3.down).eulerAngles;
	yield return new WaitForSeconds (delay);
			canFire = true;
	}


}
