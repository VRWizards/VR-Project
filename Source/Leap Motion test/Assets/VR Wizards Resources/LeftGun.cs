using UnityEngine;
using System.Collections;
using Leap;

public class LeftGun : MonoBehaviour {

	bool rockgod;
	Frame frame;
	Controller cont;
	public HandModel right;
	Hand righty;
	FingerModel[] rfingers;

	public float trigger;

	public LeapProvider lp;

	LineRenderer line;

	// Use this for initialization
	void Start () {
		rockgod = false;
		line = gameObject.GetComponent<LineRenderer>();
		line.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		frame = lp.CurrentFrame;
		if (frame.Hands.Count > 0) {
			if (frame.Hands.Leftmost.IsLeft) {
				righty = frame.Hands.Leftmost;
				rfingers = right.fingers;
			}	
		}



		if (right.isActiveAndEnabled) {
			if (Vector3.Distance (right.GetPalmPosition (), rfingers [0].GetTipPosition ()) > trigger &&
				Vector3.Distance (right.GetPalmPosition (), rfingers [1].GetTipPosition ()) > trigger &&
				Vector3.Distance (right.GetPalmPosition (), rfingers [2].GetTipPosition ()) < trigger &&
				Vector3.Distance (right.GetPalmPosition (), rfingers [3].GetTipPosition ()) < trigger &&
				Vector3.Distance (right.GetPalmPosition (), rfingers [4].GetTipPosition ()) < trigger ) {
				Debug.Log ("rockgod!!");
				rockgod = true;
			} else {
				rockgod = false;
			}
			if (rockgod) {
				line.enabled = true;
				Ray ray = new Ray (rfingers [1].GetTipPosition (), rfingers[1].GetBoneDirection(2));
//				Ray ray1 = new Ray (rfingers [4].GetTipPosition (), rfingers [4].GetBoneDirection ());

				line.SetPosition (0, ray.origin);
				line.SetPosition (1, ray.GetPoint (100));
			} else {
				line.enabled = false;
			}
		}
	}
}
