using UnityEngine;
using System.Collections;

public class ActivateChild : MonoBehaviour {

	public GameObject child;

	// Use this for initialization
	void Start () {
		child.SetActive(true);
	}

}
