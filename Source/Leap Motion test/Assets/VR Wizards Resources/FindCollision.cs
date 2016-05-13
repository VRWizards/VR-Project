using UnityEngine;
using System.Collections;
using Leap;
using ParticlePlayground;

public class FindCollision : MonoBehaviour {

	CapsuleCollider[] handColl;
	public GameObject bullet1;
	public GameObject bullet2;
	private GameObject selectedBullet;
	public Controller cont;
	public PlaygroundParticlesC thruster;
	Hand hand;
	Hand hand2;
	Frame frame;
	FingerModel[] fingers;
	FingerModel[] Lfingers;
	public float offsetx;
	public float offsety;
	public float offsetz;
	public Transform headset;
	private GameObject current;
	public HandModel righty;
	public HandModel lefty;
	public float triggerDistance;
	public float pinchDistance;
	private FingerModel fing1;	
	private FingerModel fing2;	
	private FingerModel thumb;
	public bool shooting;
	private bool shouldSwitch;
	private bool fist;
//	public bool canShoot = false;

	// Use this for initialization
	void Start () {


		shouldSwitch = true;
		selectedBullet = bullet1;
		shooting = false;
		cont = new Controller ();
		handColl = GetComponentsInChildren<CapsuleCollider> ();
		//current = Instantiate (visualizer, leapToWorld(Leap.Vector.Zero, frame.InteractionBox).ToUnityScaled() , Quaternion.identity) as GameObject;
		InvokeRepeating("ShotTimer",0,.5f);
		thruster.emit = false;
		//Debug.Log (handColl.Length);
	}
	
	// Update is called once per frame
	void Update () 
	{
		frame = cont.Frame(0);
		if (frame.Hands.Count > 0) {
//			HandList hands = frame.Hands;
//			hand = hands [0];
//			hand2 = hands [1];
			fingers = righty.fingers;
			Lfingers = lefty.fingers;
			//fingers = hand.Fingers.Extended ();
			//Finger fing1 = hand.Fingers [1];
			fing1 = fingers [1];
			fing2 = fingers [2];
			thumb = fingers [0];
		}

		for (int i = 0; i < Lfingers.Length; i++) 
		{
			if (Vector3.Distance (lefty.GetPalmPosition (), Lfingers [i].GetTipPosition ()) < triggerDistance) {
				fist = true;

			} else {
				fist = false;

			}
		}

		//Debug.Log (fist);

		//if (lefty.GetPalmRotation().x > 0.5f) {


		//Vector3 dir = lefty.GetPalmRotation () * Vector3.right;

		if (fist == false) {

			//if (lefty.
			Vector3 dir2 = -lefty.GetPalmNormal ();
			//transform.rotation = Quaternion.Lerp (transform.rotation, lefty.GetPalmRotation(), Time.deltaTime);
			transform.position = Vector3.Lerp (transform.position, transform.position + dir2 * 2, Time.deltaTime);

			thruster.emit = true;
		} else {
			thruster.emit = false;
		}

		//}

//		Debug.Log (fing1.IsExtended);

		//FingerList fingses = fingers.Extended ();

//		foreach (Finger fing in fings)
//		{
//			if (fing.Type == 
//		}
		thruster.transform.position = lefty.GetPalmPosition ();

		if (righty.isActiveAndEnabled)
		{
			if (Vector3.Distance (righty.GetPalmPosition (), fing1.GetTipPosition ()) > triggerDistance
				&& Vector3.Distance (righty.GetPalmPosition (), thumb.GetTipPosition ()) > triggerDistance) {
				//visualizer.transform.position = leapToWorld (fing1.TipPosition, frame.InteractionBox).ToUnityScaled ();
				//Debug.Log (fing1.Type);

				for (int i = 2; i < fingers.Length; i++) {
					if (Vector3.Distance (righty.GetPalmPosition (), fingers [i].GetTipPosition ()) < triggerDistance)
						shooting = true;
					else
						shooting = false;

				}

				
			} else {
				shooting = false;
			}

			if (Vector3.Distance (thumb.GetTipPosition (), fing1.GetTipPosition ()) < pinchDistance) {

				if (shouldSwitch) {
					if (selectedBullet == bullet1) {
						selectedBullet = bullet2;
					} else {
						selectedBullet = bullet1;
					}
					shouldSwitch = false;
				}
				//Debug.Log ("Could Switch");
			} else {
				shouldSwitch = true;
				//Debug.Log ("Can't Switch");
				//		}
			}
	//			if (Vector3.Distance (thumb.GetTipPosition (), fing2.GetTipPosition ()) < pinchDistance) {
	//				selectedBullet = bullet2;
	//			}
	//


		
		}
	}

	void ShotTimer()
	{
		if (shooting)
			Instantiate (selectedBullet, fing1.GetTipPosition (), fing1.GetBoneRotation (2));
	}
//
//	Leap.Vector leapToWorld(Vector3 leapPoint, InteractionBox iBox)
//	{
//		leapPoint.z *= -1.0f; //right-hand to left-hand rule
//		Leap.Vector normalized = iBox.NormalizePoint(leapPoint, false);
//		normalized += new Leap.Vector(offsetx, offsety, offsetz); //recenter origin
//		//normalized += new Leap.Vector(headset.position.x, headset.position.y, headset.position.z);
//		Vector temp = normalized;
//		//temp. = 90;
//		normalized = temp;
//		return normalized * 1f; //scale
//	}
//


//	void OnCollisionEnter(Collision coll)
//	{
//		if (coll.gameObject.tag == "canBleed") 
//		{
//		//for (int i = 0; i < handColl.Length; i++)
//
//			foreach (ContactPoint contact in coll.contacts) 
//			{
//			Instantiate (visualizer, contact.point, Quaternion.identity);
//			}
//		}
//	}


}
