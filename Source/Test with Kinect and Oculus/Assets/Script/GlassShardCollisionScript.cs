using UnityEngine;
using System.Collections;

public class GlassShardCollisionScript : MonoBehaviour {

	public AudioClip glass;

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
		if (collision.gameObject.tag == "Player") {
			guiScript.Hit = true;
		}
		AudioSource.PlayClipAtPoint(glass, transform.position);
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
		rigidbody.useGravity = true;
		//MeshCollider meshCollider = gameObject.GetComponent<MeshCollider> ();
		//meshCollider.convex = true;
		Destroy (gameObject, 15f);
	}
}
