using UnityEngine;
using System.Collections;

public class LighScript : MonoBehaviour {
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag != "Player") return;
		gameObject.transform.FindChild ("Light").gameObject.GetComponent<Light>().intensity = .5f;
		Transform contentTransform = gameObject.transform.FindChild ("Content");
		if(contentTransform != null) Destroy(contentTransform.gameObject, 30);
	}
}
