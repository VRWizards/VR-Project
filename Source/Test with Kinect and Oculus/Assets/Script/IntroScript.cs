using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour {

	public bool startTutorial = true;

	// Use this for initialization
	void Start () {	}

	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			if(startTutorial) Application.LoadLevel ("Tutorial");
			else Application.LoadLevel ("Game");
		}
	}
}
