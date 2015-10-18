using UnityEngine;
using System.Collections;

public class CrystalScript : MonoBehaviour {

	public GameObject timeBonusPrefab;
	public AudioClip audioClip;

	private GUIScript guiScript;

	// Use this for initialization
	void Start () {
		GameObject gui = Utilities.FindGameObject("GUI");
		if(gui != null) guiScript = gui.GetComponent<GUIScript>();
	}
	
	// Update is called once per frame
	void Update () {

	}


	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Player" && other.gameObject.tag != "Projectile") return;
		
		AudioSource.PlayClipAtPoint(audioClip, transform.position);
		gameObject.SetActive(false);
		foreach (GameObject child in Utilities.GetChildren(Utilities.GetParent(gameObject))) {
			if(child == this.gameObject) continue;
			child.SetActive(true);
			Destroy(child, 15f);
			if( child.GetComponent<Rigidbody>() != null) {
				child.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * Random.Range(-10, 10));
				child.GetComponent<Rigidbody>().useGravity = true;
			}
		}
		GameObject timeBonus = Instantiate (timeBonusPrefab, transform.position, new Quaternion()) as GameObject;
		Destroy (timeBonus, 2);
		if(guiScript != null) guiScript.AddBonusTime();
	}
	



}
