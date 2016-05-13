using UnityEngine;
using System.Collections;
using Leap;

public class SelfHarm : MonoBehaviour {

	CapsuleCollider[] handColl;
	public Controller cont;
	Hand hand;
	Hand hand2;
	Frame frame;
	FingerModel[] fingers;
	FingerModel[] Lfingers;
	public Transform headset;
	private GameObject current;
	public HandModel righty;
	public HandModel lefty;
	public float triggerDistance;
	public float pinchDistance;
	private FingerModel fing1;	
	private FingerModel fing2;	
	private FingerModel thumb;
	private bool shouldSwitch;
	private bool fist;
	public bool rightHanded;
	public GameObject knife;
	private Quaternion ogKnifeRot;
	private Vector3 offset;
	public static Quaternion passRot;
	public static Vector3 passPos;
	//	public bool canShoot = false;

	// Use this for initialization
	void Start () {

		offset = new Vector3 (0, .01f, 0);
		ogKnifeRot = knife.transform.rotation;
		shouldSwitch = true;
		cont = new Controller ();
		handColl = GetComponentsInChildren<CapsuleCollider> ();
		//current = Instantiate (visualizer, leapToWorld(Leap.Vector.Zero, frame.InteractionBox).ToUnityScaled() , Quaternion.identity) as GameObject;
		//InvokeRepeating("ShotTimer",0,.1f);
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

		if (!rightHanded) {

			for (int i = 0; i < Lfingers.Length; i++) {
				if (Vector3.Distance (lefty.GetPalmPosition (), Lfingers [i].GetTipPosition ()) < triggerDistance) {
					fist = true;
				} else {
					fist = false;
				}
			}
		} else if (righty.isActiveAndEnabled) {



			for (int i = 0; i < fingers.Length; i++) {
				if (Vector3.Distance (righty.GetPalmPosition (), fingers [i].GetTipPosition ()) < triggerDistance) {
					CancelInvoke();
					fist = true;

				} else {
					fist = false;
					Invoke ("StoreKnife", 2);

				}
			}

			if (lefty.isActiveAndEnabled) {
				passRot = lefty.GetArmRotation ();
				passPos = lefty.GetArmCenter ();
			}

		}

		if (fist) {
			knife.SetActive(true);



			knife.transform.position = (righty.GetPalmPosition () + righty.fingers[1].GetBoneCenter(2)) * 0.5f;
			knife.transform.rotation = righty.GetPalmRotation () * ogKnifeRot;
			MeshRenderer[] mat = knife.GetComponentsInChildren<MeshRenderer> ();
			foreach (MeshRenderer rend in mat) 
			{

				//	MeshRenderer[] mat = knife.GetComponentsInChildren<MeshRenderer> ();

				if (rend.material.color.a < 1) {
					//rend.material.color = Color.Lerp (rend.material.color, new Color (1, 1, 1, 1f), Time.deltaTime);
					rend.material.SetColor("_Color", Color.Lerp(rend.material.color,new Color (1,1,1, 1f), Time.deltaTime*3));
				} else {
					rend.material.SetFloat ("_Mode", 0);
				}
			}
		}

		//Debug.Log (fist);

		//if (lefty.GetPalmRotation().x > 0.5f) {


		//Vector3 dir = lefty.GetPalmRotation () * Vector3.right;

//		if (fist == false) {
//
//			//if (lefty.
//			Vector3 dir2 = -lefty.GetPalmNormal ();
//			//transform.rotation = Quaternion.Lerp (transform.rotation, lefty.GetPalmRotation(), Time.deltaTime);
//			transform.position = Vector3.Lerp (transform.position, transform.position + dir2 * 2, Time.deltaTime);
//		}

		//}

		//		Debug.Log (fing1.IsExtended);

		//FingerList fingses = fingers.Extended ();

		//		foreach (Finger fing in fings)
		//		{
		//			if (fing.Type == 
		//		}

			}

	void StoreKnife()
	{
		MeshRenderer[] mat = knife.GetComponentsInChildren<MeshRenderer> ();

		foreach (MeshRenderer rend in mat) 
		{
			rend.material.SetFloat ("_Mode", 2);
			if (rend.material.color.a > 0) {
				rend.material.SetColor("_Color", Color.Lerp(rend.material.color,new Color (0, 0, 0, rend.material.color.a - 0.1f), Time.deltaTime*3));
			} 
			//rend.material.SetFloat ("_Mode", 2);


//			else {
//				knife.SetActive (false);
//			}
			
		}



		//mat = Shader.Find ("Transparent");

	}



}
