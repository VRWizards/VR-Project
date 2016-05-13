using UnityEngine;
using System.Collections;
using System;
using ParticlePlayground;
//using Destruction;

public class ActivateParticle : MonoBehaviour {

	//public Cannon Gun1; 
	//public Cannon Gun2;
	//public PlaygroundParticlesC Gun2; 
	//public Cannon Gun3; 
	//private PlaygroundParticlesC Gun;
	private bool CannonOn;
	private bool WaterOn;
	private bool PlantOn;


	// Use this for initialization
	void Start () {
		CannonOn = true;
		WaterOn = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha1))
		{
//			Gun1.gameObject.SetActive(true);
			PlantOn = false;
			WaterOn = false;
			CannonOn = true;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2))
		{
//			Gun1.gameObject.SetActive(false);
			CannonOn = false;
			PlantOn = true;
			WaterOn = false;
		}
		if (Input.GetKeyDown (KeyCode.Alpha3))
		{
//			Gun1.gameObject.SetActive(false);
			CannonOn = false;
			PlantOn = false;
			WaterOn = true;
		}

//		if (CannonOn == false && WaterOn == false)
//		{
//			if (Input.GetMouseButton(0))
//			{
//				Gun.emit = true;
//			}
//			else
//			{
//				Gun.emit = false;
//			}
//		}

		//Gun1.enabled = CannonOn;
		//Gun3.enabled = WaterOn;
		//Gun2.enabled = PlantOn;


	
	}

	//void SetSelection()


}
