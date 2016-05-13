using UnityEngine;
using System.Collections;

public class TimedDestruction : MonoBehaviour {

	public float delay = 2.0f; //This implies a delay of 2 seconds.
	public float colliderDuration = 1.0f;
	private SphereCollider coll;
	
	private void Awake ()
	{
		StartCoroutine(Timer());

		coll = GetComponent<SphereCollider>();
		Destroy(this.gameObject,delay);
	}

	IEnumerator Timer()
	{
		yield return new WaitForSeconds(colliderDuration);
		coll.enabled = false;
	}
}