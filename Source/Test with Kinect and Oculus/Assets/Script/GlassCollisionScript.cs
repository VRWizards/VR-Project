using UnityEngine;
using System.Collections.Generic;

public class GlassCollisionScript : MonoBehaviour {
	
	private bool burst = true;
	private GUIScript guiScript;

	// Use this for initialization
	void Start () {
		GameObject gui = Utilities.FindGameObject("GUI");
		if(gui != null) guiScript = gui.GetComponent<GUIScript>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {

	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Player" && other.gameObject.tag != "Projectile") return;
		if (other.gameObject.tag == "Player" && guiScript != null) guiScript.AddPenalityTime ();

		gameObject.SetActive(false);
		foreach (GameObject child in Utilities.GetChildren(Utilities.GetParent(gameObject))) {
			if(child == this.gameObject) continue;
			child.SetActive(true);
			if(burst && child.GetComponent<Rigidbody>() != null) {
				child.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * Random.Range(-100, 100));
				child.GetComponent<Rigidbody>().useGravity = true;
			}
		}
	}
}
