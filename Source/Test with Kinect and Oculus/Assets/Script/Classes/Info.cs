using UnityEngine;
using System;

[Serializable]
public class Info {

	public AudioClip audio;
	public string text;

	public Info(string text, AudioClip audio) {
		this.text = text;
		this.audio = audio;
	}

}