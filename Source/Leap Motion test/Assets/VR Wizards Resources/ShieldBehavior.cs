using UnityEngine;
using System.Collections;
using Leap;

public class ShieldBehavior : MonoBehaviour {

	Frame frame;
	FingerModel[] Rfingers;
	Controller cont;
	public HandModel right;
	public Hand righty;

	public float trigger;
	public LeapProvider lp;

	bool fist;

	public Arm rightArm;

	public GameObject shield;

	bool shieldUp;
	bool shieldExists;


	// Use this for initialization
	void Start () {	
		shieldUp = false;
		fist = false;
		shieldExists = false;
	}
	
	// Update is called once per frame
	void Update () {
		frame = lp.CurrentFrame;
		if (frame.Hands.Count > 0) {
			HandList hands = frame.Hands;
			if (hands.Rightmost.IsRight) {
				Rfingers = right.fingers;
				righty = hands.Rightmost;
				rightArm = righty.Arm;
			}
		}

		if (right.isActiveAndEnabled) {
			if (Vector3.Distance (right.GetPalmPosition (), Rfingers [0].GetTipPosition ()) < trigger &&
			   Vector3.Distance (right.GetPalmPosition (), Rfingers [1].GetTipPosition ()) < trigger &&
			   Vector3.Distance (right.GetPalmPosition (), Rfingers [2].GetTipPosition ()) < trigger &&
			   Vector3.Distance (right.GetPalmPosition (), Rfingers [3].GetTipPosition ()) < trigger &&
			   Vector3.Distance (right.GetPalmPosition (), Rfingers [4].GetTipPosition ()) < trigger) {
				fist = true;
//				Debug.logger.Log ("fist true");

				if (!shieldExists) {
					Instantiate (shield, Vector3.zero, Quaternion.identity);
					shield.SetActive (false);
					shieldExists = true;
				}
				if (!shieldUp && fist == true) {
//					Debug.logger.Log ("doing shield");
					shield.SetActive(true);
//					Debug.Log (right.GetArmCenter());
					shieldUp = true;
				}
				shield = GameObject.FindGameObjectWithTag ("Shield"); 
				doShield ();
			} else {
				fist = false;
				if (shieldUp) {
//					Debug.logger.Log ("destroy about to be called");
//					Destroy (GameObject.FindGameObjectWithTag ("Shield"));
					shield.SetActive(false);
					shieldUp = false;
				}
			}
		} else {
			shield.SetActive(false);
			shieldUp = false;
		}


//		for (int i = 0; i < Rfingers.Length; i++) 
//		{
//			if (Vector3.Distance (right.GetPalmPosition (), Rfingers [i].GetTipPosition ()) < trigger) {
//				fist = true;
//				doShield ();
//				Debug.logger.Log ("fist true");
//			} else {
//				fist = false;
//				Destroy (shield);
//				shieldUp = false;
//			}
//		}
	}

	void doShield()
	{
		if (shield.activeSelf == true) {
//			Debug.logger.Log ("shield active, attempting move");
			shield.transform.rotation = Quaternion.LookRotation(righty.Arm.Direction.ToVector3());
//				Quaternion.LookRotation(new Vector3(right.GetArmDirection().x, right.GetArmDirection().y, right.GetArmDirection().z));
//				Quaternion.LookRotation(new Vector3 (right.forearm.rotation.x, right.forearm.rotation.y, -right.forearm.rotation.z)); 
			shield.transform.position = righty.Arm.Center.ToVector3 ();
//					new Vector3 (right.forearm.position.x, right.forearm.position.y, -right.forearm.position.z);
		}
	}
}
