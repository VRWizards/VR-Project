using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	public GUIStyle gStyle;
	public Texture texture;
	public AudioClip taskSolvedAudio;
	public float scale = 0.5f;
	public Vector2 offset = new Vector2(0,0);

	public SpriteSheet speedMeter;
	public int SpeedMeter {get; set;}

	//private float duration  = 1.0f;
	private int rightHandTrigger;
	private int leftHandTrigger;
	private int bothHandTrigger;
	private int turnHorizontalTrigger;
	private int turnVerticalTrigger;
	private int idleTrigger;

	private ShootScript shootScript;
	private FlightScript flightScript;
	private bool understoodFlying;
	private bool understoodShooting;
	private bool understoodTurnHorizontal;
	private bool understoodTurnVertical;

	private GameObject gui;
	private GameObject camera;
	private Animator animator;
	private GameObject crystals;
	private GameObject[] listOfCrystals;
	private int iterations = 3;
	
	public State state = State.IDLE;
	public enum State {IDLE,INITIALIZE,SHOOT,ACCELERATE,TURN_HORIZONTAL,TURN_VERTICAL,TEST_TRACK,DONE}

	private State[] stateOrder = {State.INITIALIZE,State.ACCELERATE,State.TURN_HORIZONTAL,State.TURN_VERTICAL,State.SHOOT,State.TEST_TRACK,State.DONE};
	private int stateIndex = 0;
	private float idleTime = 3;
	private float time = -1;

	private int hits = 0;
	private float currentTargetSpeed = float.MaxValue;
	private int accelerations = 0;
	private bool waitForHighAcceleration = true;
	private bool waitForCompleteRotation = false;
	private bool waitForUpwards = true;

	string welcomeText = "Wilkommen zur Einführung!";
	string accelerateText = "Strecke die rechte Hand vor um deine Geschwindigkeit zu regeln.\n";
	string turnHeadHorizontalText = "Bewege deinen Kopf nach rechts/links um dich umzudrehen.\n";
	string turnHeadVerticalText = "Bewege deinen Kopf nach oben/unten um dich umzusehen.\n";
	string shootText = "Strecke die linke Hand vor um zu schießen. Zerstöre die Kristalle.\n";
	string beginTestTrackText = "Bevor es richtig los geht, folgt nun ein kurzer Testlauf.\n";
	string doneText = "Einführung erfolgreich beendet. Mach dich bereit zu spielen...";

	private KinectPointController kinect;
	private string text;
	private int initializeInfo = 0;

	public AudioClip welcomeAudio;
	public Info[] initializeInfos;
	public AudioClip accelerateAudio;
	public AudioClip shootAudio;
	public AudioClip turnHeadHorizontalAudio;
	public AudioClip turnHeadVerticalAudio;
	public AudioClip doneAudio;
	public AudioClip againAudio;
	public AudioClip[] wellDoneAudios;
	public Info startTestTrackInfo;
	public Info reachTunnelEndAudio;

	public void Start() {
		gui = Utilities.FindGameObject("GUI");
		kinect = GameObject.Find("Player").GetComponent<KinectPointController>();
		GameObject sara = GameObject.Find("Sara");
		sara.transform.Rotate(Vector3.left, 90);
		sara.transform.position += new Vector3(0,9,0);
		camera = GameObject.Find("CameraRight");
		animator = sara.GetComponent<Animator> ();
		rightHandTrigger = Animator.StringToHash("right");
		leftHandTrigger = Animator.StringToHash("left");
		bothHandTrigger = Animator.StringToHash("both");
		turnHorizontalTrigger = Animator.StringToHash("horizontal");
		turnVerticalTrigger = Animator.StringToHash("vertical");
		idleTrigger = Animator.StringToHash("idle");
		GameObject player = GameObject.Find("Player");
		crystals = GameObject.Find ("Crystals");
		listOfCrystals = GameObject.FindGameObjectsWithTag ("Crystal");
		crystals.SetActive (false);
		shootScript = player.GetComponent<ShootScript> ();
		flightScript = player.GetComponent<FlightScript> ();
		//shootScript.Shoot += new ShootEventHandler(HasShot);
		flightScript.Flown += new FlightEventHandler(HasFlown);
		//GameObject.Find("CameraRight").camera.backgroundColor = Color.white;
		text = welcomeText;
		playSound(welcomeAudio);
	}

	private bool newState; 

	public void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("Game");
			return;
		}
		switch (state) {
			case State.IDLE: 
				if(time == -1) time = Time.time;
				if(Time.time - time >= idleTime){
					idleTime = 3;
					time = -1;
					if(stateIndex >= stateOrder.Length){
						Application.LoadLevel("Game");
						return;
					}
					state = stateOrder[stateIndex];
					stateIndex++;
					newState = true;
				}
				animator.SetTrigger (idleTrigger);
				return;
			case State.INITIALIZE: 
				// TODO check start position
				if (GetComponent<AudioSource>().clip == null || !GetComponent<AudioSource>().isPlaying) {
					if(initializeInfo == 1 && kinect.Shoulder_Left.transform.position.Equals (kinect.Shoulder_Right.transform.position)) break;
					if(initializeInfo >= initializeInfos.Length) {
						idleTime = 0;
						if(currentTargetSpeed == 0) state = State.IDLE;
					} else {
						GetComponent<AudioSource>().clip = initializeInfos [initializeInfo].audio;
						GetComponent<AudioSource>().PlayDelayed (0.5f);
						text = initializeInfos [initializeInfo].text;
						initializeInfo++;
					}
				}
				break;
			case State.SHOOT:
				shootScript.enableShooting = true;
				crystals.SetActive(true);
				int hits = 0;
				foreach(GameObject crystal in listOfCrystals) {
					if(!crystal.activeSelf) hits++;
				}
				if(hits != this.hits && hits < iterations) playSound(againAudio);
				this.hits= hits;
				if(hits >= iterations) understoodShooting = true;
				text = shootText + "(" + hits + "/" + iterations + ")";
				if(newState) playSound(shootAudio);
				animator.SetTrigger (leftHandTrigger);
				if (Input.GetKeyDown (KeyCode.Space)) understoodShooting = true;
				if(understoodShooting){
					understood(shootText + "(OK)");
				}
				break;
			case State.ACCELERATE:
				//flightScript.enableMovement = true;
				text = accelerateText +"(" + accelerations + "/"+iterations+")";
				if(newState) playSound(accelerateAudio);
				animator.SetTrigger (rightHandTrigger);
				if (Input.GetKeyDown (KeyCode.Space)) understoodFlying = true;
				if(understoodFlying){
					understood(accelerateText + "(OK)");
				}
				break;
			case State.TURN_HORIZONTAL: 
				flightScript.enableRotation = true;
				animator.SetTrigger (turnHorizontalTrigger);
				text = turnHeadHorizontalText;
				if(newState) playSound(turnHeadHorizontalAudio);
				if (Input.GetKeyDown (KeyCode.Space)) understoodTurnHorizontal = true;
				if(understoodTurnHorizontal){
					understood(turnHeadHorizontalText+"(OK)");
				}
				break;
			case State.TURN_VERTICAL: 
				animator.SetTrigger (turnVerticalTrigger);
				text = turnHeadVerticalText;
				if(newState) playSound(turnHeadVerticalAudio);
				checkHasUnderstoodTurnVertical();
				if (Input.GetKeyDown (KeyCode.Space)) understoodTurnVertical = true;
				if(understoodTurnVertical){
					understood(turnHeadVerticalText + "(OK)");
				}
				break;
		case State.TEST_TRACK:
				if(newState) {
					GameObject.Find("Start/Back").SetActive(false);
					flightScript.enableMovement = true;
					text = startTestTrackInfo.text;
					GetComponent<AudioSource>().clip = startTestTrackInfo.audio;
					GetComponent<AudioSource>().Play();
				}
				if(!GetComponent<AudioSource>().isPlaying && text == startTestTrackInfo.text) {
					text = "";
				}
				gui.SetActive(true);
				break;
			case State.DONE:
				idleTime = 3;
				gui.SetActive(false);
				flightScript.enableMovement = false;
				shootScript.enableShooting = false;
				text = doneText;
				if(newState) playSound(doneAudio);
				idleTime = 5;
				state = State.IDLE;
				break;
		}
		newState = false;
	}

	private void playSound(AudioClip audioClip, float delay = 0.0f){
		if (GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop ();
		GetComponent<AudioSource>().clip = audioClip;
		GetComponent<AudioSource>().PlayDelayed (delay);
	}

	private void understood(string text){
		AudioSource.PlayClipAtPoint(taskSolvedAudio, new Vector3());
		int index = Random.Range (0, wellDoneAudios.Length - 1);
		playSound(wellDoneAudios[index]);
		this.text = text;
		state = State.IDLE;
	}

//	private void HasShot(object sender, EventArgs arguments) {
//		if (state != State.SHOOT) return;
//		Debug.Log ("Shots: " + hits);
//		hits++;
//		if (hits >= 3) understoodShooting = true;
//	}

	private void HasFlown(object sender, FlightEventArgs arguments) {
		currentTargetSpeed = arguments.TargetSpeed;
		if (state == State.ACCELERATE) {
			//Debug.Log ("Target Speed: " + arguments.TargetSpeed + " Accelerations: " + arguments.TargetSpeed);
			if (waitForHighAcceleration) {
				if (arguments.TargetSpeed > arguments.MaxSpeed * 0.8f) {
					accelerations++;
					if(accelerations < iterations) playSound(againAudio);
					waitForHighAcceleration = false;
				}
			} else {
				if (arguments.TargetSpeed < arguments.MaxSpeed / 4)	waitForHighAcceleration = true;
			}
			if (accelerations >= iterations)	understoodFlying = true;
		} else if (state == State.TURN_HORIZONTAL) {
			//Debug.Log (arguments.Rotation);
			if (waitForCompleteRotation) {
				if (Mathf.Abs (arguments.Rotation) < 15) understoodTurnHorizontal = true;
			} else {
				if (arguments.Rotation > 90 || arguments.Rotation < -90) {
					waitForCompleteRotation = true;
				}
			}
		}
	}

	private void checkHasUnderstoodTurnVertical() {
		Vector3 oculusForward = camera.transform.forward.normalized;
		Vector3 vector = ProjectOnPlane (oculusForward, Vector3.up, Vector3.forward);
		float sign = Vector3.Dot (vector, Vector3.up);
		float angle = sign * Vector3.Angle (vector, Vector3.forward);
		//Debug.Log (angle +"°");
		if (waitForUpwards) {
			if(angle > 30) waitForUpwards = false;
		} else {
			if(angle < -30)	understoodTurnVertical = true;
		}
	}

	private Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeX, Vector3 planeY) {
		Vector3 normal = Vector3.Cross (planeX, planeY).normalized;
		return vector - Vector3.Dot (vector, normal) * normal;
	}

	public void setInfo(Info info) {
		if (info.audio == null) { 
			state = State.IDLE;
			idleTime = 1;
			return;
		}
		playSound(info.audio);
		text = info.text;
	}


	public void OnGUI(){
		Utilities.Label (text, offset.x, offset.y, gStyle);

		if (state == State.ACCELERATE )speedMeter.Render(SpeedMeter);
		if (state == State.SHOOT) Utilities.Label ("+", 0, 0, gStyle);
//		float lerp = Mathf.PingPong (Time.time, duration)/duration;
//		scale =Mathf.Lerp (0.2f, 0.4f, lerp);
//		float width = texture.width * scale;
//		float height = texture.height * scale;
//		GUI.DrawTexture (new Rect (Screen.width * 0.3f - width/2 + offset.x, Screen.height/2 - height/2 + offset.y, width, height), texture);
//		GUI.DrawTexture (new Rect (Screen.width * 0.7f - width/2 + offset.x, Screen.height/2 - height/2 + offset.y, width, height), texture);
//		Vector2 textSize = gStyle.CalcSize (new GUIContent (text));
//		GUI.Label(new Rect(Screen.width * 0.3f + offset.x - textSize.x/2, Screen.height/2 + offset.y  , 200, 200), text, gStyle);
//		GUI.Label(new Rect(Screen.width * 0.7f + offset.x - textSize.x/2, Screen.height/2 + offset.y , 200, 200), text, gStyle);
	}
	
}
