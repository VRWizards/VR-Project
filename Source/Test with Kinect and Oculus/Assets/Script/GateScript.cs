using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class GateScript : MonoBehaviour {

	public enum State {Opening, Opened, Closing, Closed};

	public State state = State.Opened;
	public bool stopped = false;
	public float openingFactor = 1;
	public float closingFactor = 1;
	public float initialDelay = 0;
	public float openingDuration = 1;
	public float openedDuration = 1;
	public float closingDuration = 1;
	public float closedDuration = 1;
	public GameObject left;
	public GameObject right;

	private float delay = 0;
	private float time = 0;
	private Vector3 leftOpened;
	private Vector3 leftClosed;
	private Vector3 rightOpened;
	private Vector3 rightClosed;

	private float validOpeningFactor;
	private float validClosingFactor;
	private Quaternion validRotation;
	
	// Use this for initialization
	void Start () {
		if (initialDelay < 0) initialDelay = Random.Range (0, -initialDelay);
		if (openingDuration < 0) openingDuration = Random.Range (0, -openingDuration);
		if (openedDuration < 0) openedDuration = Random.Range (0, -openedDuration);
		if (closingDuration < 0) closingDuration = Random.Range (0, -closingDuration);
		if (closedDuration < 0) closedDuration = Random.Range (0, -closedDuration);
		UpdatePositions ();
		delay = initialDelay;
	}

	void UpdatePositions() {
		validRotation = transform.rotation;
		validOpeningFactor = openingFactor;
		validClosingFactor = closingFactor;
		Vector3 halfLeft = -left.transform.right * left.transform.localScale.x / 2;
		Vector3 halfRight = right.transform.right * right.transform.localScale.x / 2;
		leftClosed = transform.position + halfLeft / closingFactor;
		rightClosed = transform.position + halfRight / closingFactor;
		leftOpened = transform.position + halfLeft * 3 * openingFactor;
		rightOpened = transform.position + halfRight * 3 * openingFactor;
		bool initiallyOpen = state == State.Opened || state == State.Closing;
		left.transform.position = initiallyOpen ? leftOpened : leftClosed;
		right.transform.position = initiallyOpen ? rightOpened : rightClosed;
	}

	bool ValidPositions() {
		return transform.rotation == validRotation && openingFactor == validOpeningFactor && closingFactor == validClosingFactor;
	}
	
	// Update is called once per frame
	void Update () {
		if (!ValidPositions()) UpdatePositions ();
		if (stopped) return;
		if (delay > 0) {
			delay -= Time.deltaTime;
			if (delay >= 0) return;
			time = -delay;
			delay = 0;
		} else {
			time += Time.deltaTime;
		}
		switch (state) {
		case State.Opening:
			Open ();
			if(time >= openingDuration) {
				state = State.Opened;
				time -= openingDuration;
			}
			break;
		case State.Opened:
			if(time >= openedDuration) {
				state = State.Closing;
				time -= openedDuration;
			}
			break;
		case State.Closing:
			Close ();
			if(time >= closingDuration) {
				state = State.Closed;
				time -= closingDuration;
			}
			break;
		case State.Closed:
			if(time >= closedDuration) {
				state = State.Opening;
				time -= closedDuration;
			}
			break;
		}
	}

	private void Open() {
		float fraction = Mathf.Min (time / openingDuration, 1);
		left.transform.position = Vector3.Lerp(leftClosed, leftOpened, fraction);
		right.transform.position = Vector3.Lerp(rightClosed, rightOpened, fraction);
	}

	private void Close() {
		float fraction = Mathf.Min (time / closingDuration, 1);
		left.transform.position = Vector3.Lerp(leftOpened, leftClosed, fraction);
		right.transform.position = Vector3.Lerp(rightOpened, rightClosed, fraction);
	}

	// FixedUpdate is called every 0.02 seconds right before physics calculation
	void FixedUpdate () {}

}
