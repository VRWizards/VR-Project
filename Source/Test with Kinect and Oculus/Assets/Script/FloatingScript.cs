using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatingScript : MonoBehaviour {

	public float duration = 1;
	public float delay = 0;
	public Vector3 distance;
	public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
	public bool smoothStep = true;
	public bool stopOnCollision = true;
	public bool stopped = false;

	private float time;
	private List<GameObject> children;
	private Dictionary<GameObject, Vector3> origins = new Dictionary<GameObject, Vector3>();
	private GameObject complete;

	// Use this for initialization
	void Start () {
		float magnitude = distance.magnitude;
		distance = transform.localToWorldMatrix.MultiplyVector (distance).normalized * magnitude;
		children = Utilities.GetChildren (gameObject);
		foreach (GameObject child in children) {
			if(child.activeSelf) complete = child;
			origins.Add(child, child.transform.position);
		}
		time = duration/2;
	}
	
	// Update is called once per frame
	void Update () {
		if (stopped) return;
		if (stopOnCollision && !complete.activeSelf) stopped = true;
		if (delay > 0) {
			delay -= Time.deltaTime;
			if (delay >= 0) return;
			time += -delay;
			delay = 0;
		} else {
			time += Time.deltaTime;
		}
		bool reset = distance.magnitude == 0;
		float fraction = 0;
		if (!reset) {
			fraction = curve.Evaluate(Mathf.PingPong(time, duration) / duration);
			if (smoothStep) fraction = Mathf.SmoothStep(0, 1, fraction);
		}

		foreach (GameObject child in children) {
			if (child == null || child.GetComponent<Rigidbody>() != null && child.GetComponent<Rigidbody>().useGravity) continue;
			Vector3 origin = origins[child];
			Vector3 position = reset? origin : Vector3.Lerp(origin + distance, origin - distance, fraction);
			child.transform.position = position;
		}
	}

}
