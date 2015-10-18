using UnityEngine;
using System;
using System.Collections;

public class FlightEventArgs : EventArgs
{
	public FlightEventArgs(float speed, float targetSpeed, float maxSpeed, float rotation) {
		Speed = speed;
		TargetSpeed = targetSpeed;
		MaxSpeed = maxSpeed;
		Rotation = rotation;
	}
	public float Speed { get; set; }
	public float TargetSpeed { get; set; }
	public float MaxSpeed { get; set; }
	public float Rotation { get; set; }
}
public delegate void FlightEventHandler(object sender, FlightEventArgs e);

public class FlightScript : MonoBehaviour {

	public event FlightEventHandler Flown;

	private static Vector3 XAxis = new Vector3(1,0,0);
	private static Vector3 YAxis = new Vector3(0,1,0);
	private static Vector3 ZAxis = new Vector3(0,0,1);
	private static float FistChestOffset = 0.4f;
	private static float SpeedFactor = 1000.0f;
	private static float Acceleration = 100.0f;
	private static float Deceleration = -150.0f;
	private static float MaxSpeed = 200.0f;

	public bool enableMovement = true;
	public bool enableRotation = true;

	private GameObject camera;	
	private KinectPointController controller;
	private Vector3 chest;
	private GameObject hip;
	private Vector3 startPos;
	private float speed;

	private GUIScript guiScript;
	private Tutorial tutorialScript;
	
	// Use this for initialization
	void Start() {
		controller = this.GetComponent<KinectPointController>();
		camera = GameObject.Find("CameraRight");
		GameObject guiTutorial = GameObject.Find ("GUI-Tutorial");
		if (guiTutorial != null) tutorialScript = guiTutorial.GetComponent<Tutorial> ();
		GameObject gui = Utilities.FindGameObject("GUI");
		if(gui != null) guiScript = gui.GetComponent<GUIScript>();
		hip = controller.Hip_Center;

		//KinectPointController
		startPos = this.transform.position;
		speed = 0.0f;
	}

	// Update is called once per frame
	void Update () {
		chest = (controller.Shoulder_Left.transform.position + controller.Shoulder_Right.transform.position) / 2;
		Vector3 direction = controller.Wrist_Right.transform.position - chest;
		//Debug.Log ("Magnitude: " + direction.magnitude);
		float delta = Mathf.Max(0, direction.magnitude - FistChestOffset);
		float targetSpeed = delta * SpeedFactor;
		//Debug.Log ("Target Speed: " + targetSpeed);


		bool reset = Input.GetKeyDown(KeyCode.R);
		if (reset) {
			this.transform.position = startPos;
			this.transform.rotation = new Quaternion (0, 0, 0, 0);
		} else {
			Vector3 oculusForward = camera.transform.forward.normalized;
			Vector3 chestToHip = hip.transform.position - chest;
			Vector3 chestToShoulder = controller.Shoulder_Left.transform.position - chest;
			Vector3 forward = Vector3.Cross(chestToShoulder, chestToHip);
			
			Debug.DrawRay(chest, forward.normalized * 100, Color.blue);
			Debug.DrawRay(chest, oculusForward * 100, Color.red);

			Vector3 forwardOnPlane = ProjectOnPlane(forward, XAxis, ZAxis);			
			Vector3 oculusForwardOnPlane = ProjectOnPlane(oculusForward, XAxis, ZAxis);			
			float angle = Degrees(Angle(oculusForwardOnPlane, forwardOnPlane));
			if(enableRotation && angle > 30) {
				float rotationDirection = Mathf.Sign(Vector3.Dot(oculusForwardOnPlane, chestToShoulder));
				if(rotationDirection > 0) {
					Debug.DrawRay(chest, oculusForwardOnPlane.normalized * 100, Color.magenta);
				} else {
					Debug.DrawRay(chest, oculusForwardOnPlane.normalized * 100, Color.cyan);
				}
				angle -= 30;
				angle *= -rotationDirection;
				//Debug.Log(angle + "°");
				this.transform.RotateAround(this.transform.position, YAxis, angle * 0.1f);
			}

			speed = GetComponent<Rigidbody>().velocity.magnitude;
			if(guiScript != null) {
				guiScript.Speed = speed;
				guiScript.SpeedMeter = (int) Mathf.Min(99, targetSpeed/MaxSpeed * 100);
			}
			if(tutorialScript != null) {
				tutorialScript.SpeedMeter = (int) Mathf.Min(99, targetSpeed/MaxSpeed * 100);
			}
			if(Flown != null) Flown(this, new FlightEventArgs(speed, targetSpeed, MaxSpeed, transform.rotation.eulerAngles.y));
			//Debug.Log("speed target: " + targetSpeed+ " velocity: " + this.rigidbody.velocity.magnitude);
			if(targetSpeed != speed) {
				if(equals(speed, targetSpeed, 0.1f)) {
				//if(Mathf.Abs(targetSpeed - speed) < 10) {
					speed = targetSpeed;
				} else {
					float acceleration = (speed < targetSpeed)? Acceleration : Deceleration;
					speed = Mathf.Max(0, Mathf.Min(MaxSpeed, speed + acceleration * Time.deltaTime));
				}
			}
			if (enableMovement) {
				this.GetComponent<Rigidbody>().velocity = oculusForward * speed;
			} else {
				this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}

	Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeX, Vector3 planeY) {
		Vector3 normal = Vector3.Cross (planeX, planeY).normalized;
		return vector - Vector3.Dot (vector, normal) * normal;
	}
	
	float Angle(Vector3 a, Vector3 b) {
		float n = Vector3.Dot(a, b) / (a.magnitude * b.magnitude);
		if (n < -1.0f) n = -1.0f;
		else if (n > 1.0f) n = 1.0f;
		return Mathf.Acos (n);
	}

	float Degrees(float radian) {
		return radian * 180 / Mathf.PI;
	}
	
	bool equals(float d1, float d2, float precision)
	{
		float eps1 = Mathf.Abs(d1), eps2 = Mathf.Abs(d2), eps;
		eps = (eps1 > eps2) ? eps1 : eps2;
		if (eps == 0.0f)
			return true; //both numbers are 0.0
		//eps hold the minimum distance between the values
		//that will be considered as the numbers are equal
		//considering the magnitude of the numbers
		eps *= precision;
		return (Mathf.Abs(d1 - d2) < eps);
	}
	
}
