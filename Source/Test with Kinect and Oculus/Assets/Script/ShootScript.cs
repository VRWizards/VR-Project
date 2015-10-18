using UnityEngine;
using System;
using System.Collections;

public delegate void ShootEventHandler(object sender, EventArgs e);

public class ShootScript : MonoBehaviour {

	public event ShootEventHandler Shoot;

	public Rigidbody projectilePrefab;
	public float projectileForce = 10000;
	public GameObject camera;

	public bool enableShooting = true;

	private KinectPointController controller;

	Color colorStart  = Color.red; 
    Color colorEnd  = Color.green; 
    float duration  = 1.0f;
	float deltaTime = 0f;
    Vector3 oldPosition;
	bool doColorize = false;
	private GameObject projectiles;
	
	void Start() {
		projectiles = new GameObject ("Projectiles");
		controller = this.GetComponent<KinectPointController> ();
	}

	// Update is called once per frame
	void Update () {
		Vector3 direction = controller.Shoulder_Left.transform.position - controller.Wrist_Left.transform.position;
		//colorize ();
		deltaTime += Time.deltaTime;
		if (direction.magnitude > 0.4 && deltaTime > 0.5f) {
			deltaTime = 0f;
			if(Shoot != null) Shoot(this, EventArgs.Empty);
			if (!enableShooting)	return;
			Rigidbody projectile = Instantiate (projectilePrefab, camera.transform.position + camera.transform.forward * 10, camera.transform.rotation) as Rigidbody;
			projectile.transform.parent = projectiles.transform;
			projectile.velocity = GetComponent<Rigidbody>().velocity;
			GetComponent<Rigidbody>().velocity /= 2;
			projectile.AddForce (camera.transform.forward * projectileForce);
			Destroy(projectile.transform.gameObject, 10);
		}
	}

	private void colorize(){
		float lerp = Mathf.PingPong (Time.time, duration) / duration;
		projectilePrefab.GetComponent<Renderer>().material.color = Color.Lerp (colorStart, colorEnd, lerp);
	}

}
