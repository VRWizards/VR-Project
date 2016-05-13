using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class MirrorEmission : MonoBehaviour {

	private PlaygroundParticlesC thisPart;
	public PlaygroundParticlesC thatPart;

	void Start()
	{
		thisPart = GetComponent<PlaygroundParticlesC> ();
		//thatPart = GetComponentInParent<PlaygroundParticlesC> ();
	}
	// Update is called once per frame
	void Update () {
		thisPart.emit = thatPart.emit;
	}
}
