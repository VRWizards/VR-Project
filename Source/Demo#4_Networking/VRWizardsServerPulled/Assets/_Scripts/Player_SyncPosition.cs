using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_SyncPosition : NetworkBehaviour {

	[SyncVar]
	private Vector3 syncPos;

	[SerializeField] Transform myTransform;
	[SerializeField] float lerpRate = 15;

	// Use this for initialization
	void Start () {

		myTransform = this.transform;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		TransmitPosition ();
		LerpPosition ();
	
	}

	void LerpPosition()
	{
		if (!isLocalPlayer) 
		{
			myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdProvidePositionToServer (Vector3 pos)
	{
		syncPos = pos;
	}

	[ClientCallback]
	void TransmitPosition()
	{
		CmdProvidePositionToServer (myTransform.position);
	}
}
