﻿using UnityEngine;
using System.Collections;

public class TutorialObjectiveScript : MonoBehaviour {

	public Info info;

	private Tutorial tutorial;

	// Use this for initialization
	void Start () {
		tutorial = GameObject.Find("GUI-Tutorial").GetComponent<Tutorial> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Player") return;
		tutorial.setInfo (info);
	}
}
