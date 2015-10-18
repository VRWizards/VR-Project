using UnityEngine;
using System;
using System.Collections;

public class GUIScript : MonoBehaviour {

	public enum State {Running, GameOver, Success}
	public enum Alignment {Left, Center, Right}

	public AudioClip alert;
	public AudioClip[] countdown;

	public AudioClip gameOverAudioClip;
	public AudioClip gameFinishAudioClip;
	public AudioClip panaltyAudioClip;

	public GUIStyle gStyle;
	public SpriteSheet shatteringGlass;
	public SpriteSheet speedMeter;
	public GameObject victoryBox;

	public State GameState {get; set;}
	public bool Hit {get; set;}
	public string CenterText {get; set;}
	public float GameTime = 60;
	public float Speed {get; set;}
	public int SpeedMeter {get; set;}

	private AudioSource backgroundAudio;
	private Tutorial tutorial;
	private int second;
	private float fadeTime;

	private GameObject camera;
	private GameObject player;
	private GameObject finish;
	private Light light;

	public bool showInfos = false;
	public Info[] infos;
	private int infoIndex = 0;

	void Start(){
		backgroundAudio = GameObject.Find ("BackgroundMusic").GetComponent<AudioSource> ();
		tutorial = GetComponent<Tutorial>();
		camera = GameObject.Find("CameraRight");
		player = GameObject.FindWithTag ("Player");
		finish = GameObject.FindWithTag("Finish");
		if(finish != null) light = finish.transform.FindChild("Light").gameObject.GetComponent<Light>();
		Hit = false;
		CenterText = "+";
		Speed = 0;
		SpeedMeter = 0;
	}

	void Update () {
		GameTime -= Time.deltaTime;
	}
	
	void OnGUI(){
		switch (GameState) {
		case State.Running:
			CenterText = "+";
			WhileRunning();
			break;
		case State.Success:
			light.color = Color.Lerp(Color.green, Color.white, Time.time - fadeTime);
			light.range += Time.deltaTime * 10;
			light.intensity += Time.deltaTime / 10;
			backgroundAudio.volume -= Time.deltaTime / 10;
			CenterText = "Success";
			break;
		case State.GameOver:
			backgroundAudio.volume -= Time.deltaTime / 2;
			CenterText = "Game Over";
			break;
		default:
			break;
		}
		Utilities.Label(CenterText, 0, 0, gStyle);
	}

	void WhileRunning() {
		//if (showInfos) showInfo ();
		//else infoIndex = 0;
		speedMeter.Render(SpeedMeter);
		if (Hit) {
			shatteringGlass.Render();
			Hit = !shatteringGlass.IsFinished ();
		}
		if(GameTime <= 0) {
			GameTime = 0;
			OnGameOver();
		} else if (GameTime < 15) {
			int time = (int) Time.time;
			if(time > second) {
				AudioSource.PlayClipAtPoint(alert, camera.transform.position);
				if( (int)GameTime < countdown.Length)
					AudioSource.PlayClipAtPoint(countdown[(int)GameTime],new Vector3());
				second = time;
			}
			gStyle.normal.textColor = gStyle.focused.textColor;
		} else {
			gStyle.normal.textColor = gStyle.active.textColor;
		}
		Utilities.Label(Utilities.TimeToString(GameTime), 0, -190, gStyle);
		Vector2 textSize = gStyle.CalcSize (new GUIContent ("000 km/h"));
		Utilities.Label (Utilities.SpeedToString(Speed), textSize.x/2, 190, gStyle, Utilities.Alignment.Right);
	}

	void showInfo() {
		if (infoIndex < infos.Length) {
			backgroundAudio.volume = 0.25f;
			if (GetComponent<AudioSource>().clip == null || !GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().clip = infos [infoIndex].audio;
				GetComponent<AudioSource>().PlayDelayed (0.5f);
				infoIndex++;
			}
		} else if(!GetComponent<AudioSource>().isPlaying) {
			backgroundAudio.volume = 0.5f;
		}
	}

	public void OnSuccess() {
		AudioSource.PlayClipAtPoint (gameFinishAudioClip, new Vector3 ());
		finish.transform.parent = null;
		foreach (GameObject child in Utilities.GetChildren (finish)) {
			child.SetActive(true);
		}
		GameObject.Find ("Track").SetActive(false);
		GameObject.Find ("Particles").SetActive (false);
		FlightScript flightScript = player.GetComponent<FlightScript>();
		ShootScript shootScript = player.GetComponent<ShootScript>();
		flightScript.enabled = false;
		shootScript.enabled = false;
		fadeTime = Time.time;
		GameState = State.Success;
	}

	public void OnGameOver() {
		AudioSource.PlayClipAtPoint (gameOverAudioClip, new Vector3 ());
		GameObject.Find ("Track").SetActive (false);
		GameState = State.GameOver;
	}

	public void AddBonusTime() {
		GameTime += 10;
	}

	public void AddPenalityTime() {
		GameTime -= 5;
		AudioSource.PlayClipAtPoint (panaltyAudioClip, new Vector3 ());
	}

}
