using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {

	public GameObject child;
	public FindCollision logic;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) 
		{
			logic = GetComponent<FindCollision> ();
			logic.enabled = true;
			child.SetActive (true);
		}
	}
	

}
