using UnityEngine;
using System.Collections;

public class Obstacle1Mover : MonoBehaviour {
	float duration  = 1.0f;

	public float min = 0;
	public float max = 100;
	public Vector3 axis = new Vector3(0,1,0);
	private Vector3 startPos;


	// Use this for initialization
	void Start () {
		startPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float value = getOffset ();
		this.transform.position = startPos + axis*value;
	}


	private float getOffset(){
		float lerp = Mathf.PingPong (Time.time, duration) / duration;
		return Mathf.Lerp (min, max, lerp);
	}
}
