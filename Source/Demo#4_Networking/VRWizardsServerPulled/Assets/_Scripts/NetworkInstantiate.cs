using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class NetworkInstantiate : NetworkBehaviour {

	public List<FindCollision> playerList;
	private GameObject[] players;

	void Start()
	{
		playerList = new List<FindCollision> ();
	}
	
	// Update is called once per frame
	void Update () {

		playerList.Clear ();
		players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject coll in players) {
			playerList.Add(coll.GetComponent<FindCollision>());
		}


		if (playerList.Count > 0) {
		//	if (NetworkServer.active) {
				foreach (FindCollision player in playerList) {
					if (player.shooting) {
						//InvokeRepeating ("CmdShotTimer", 0, 0.2f);
						//CmdShotTimer();
						GameObject go = Instantiate (player.selectedBullet, player.fing1.GetTipPosition (), player.fing1.GetBoneRotation (2)) as GameObject;
						NetworkServer.Spawn (go);
					} //else {

					//	}
		//		}
			}

			Debug.Log (playerList.Count);
		}
	
	}

//	void OnPlayerConnected(NetworkPlayer player)
//	{
//		playerList.Clear ();
//		players = GameObject.FindGameObjectsWithTag ("Player");
//		foreach (GameObject coll in players) {
//			playerList.Add(coll.GetComponent<FindCollision>());
//		}
//
//	}

//	[Command]
//	void CmdShotTimer()
//	{
////	if (shooting && NetworkServer.active == true) {
//			GameObject go = Instantiate (selectedBullet, fing1.GetTipPosition (), fing1.GetBoneRotation (2)) as GameObject;
//			NetworkServer.Spawn(go);
//	//	}
//	}
}
