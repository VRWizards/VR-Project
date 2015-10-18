using UnityEngine;
using System.Collections;

public class GoalScript : MonoBehaviour {

	private GUIScript guiScript;
	
	// Use this for initialization
	void Start () {
		guiScript = GameObject.Find ("GUI").GetComponent<GUIScript>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Player") return;
		guiScript.OnSuccess();
	}

}
