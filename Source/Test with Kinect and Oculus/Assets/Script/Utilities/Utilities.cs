using UnityEngine;
using System;
using System.Collections.Generic;

public class Utilities {

	public enum Alignment {Left, Center, Right}
	
	private Utilities() {
		throw new System.Exception ("Utility classes may not be instantiated.");
	}

	public static GameObject FindGameObject (string name) {

		foreach (GameObject o in (GameObject[])	Resources.FindObjectsOfTypeAll (typeof(GameObject)) ) {
			if (o.name == name) {
				return o;
			}
		}

		return null;
	}

	public static List<GameObject> GetChildren(GameObject gameObject) {
		List<GameObject> children = new List<GameObject>();
		foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true)) {
			if (transform != gameObject.transform) children.Add(transform.gameObject);
		}
		return children;
	}

	public static GameObject GetParent(GameObject gameObject) {
		return gameObject.transform.parent.gameObject;
	}

	public static string TimeToString(float time) {
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		string sign = (timeSpan.Minutes < 0 || timeSpan.Seconds < 0 || timeSpan.Milliseconds < 0) ? "-" : " ";
		return string.Format(sign + "{0:D2}:{1:D2}:{2:D2} ", Mathf.Abs(timeSpan.Minutes), Mathf.Abs(timeSpan.Seconds), Mathf.Abs(timeSpan.Milliseconds / 10));
	}
	
	public static string SpeedToString(float speed) {
		return Mathf.RoundToInt(speed) + " km/h";
	}
	
	public static void Label(string text, float x, float y, GUIStyle style, Alignment alignment = Alignment.Center, bool border = true, Color borderColor = new Color()) {
		Vector2 textSize = style.CalcSize (new GUIContent (text));
		Vector2 offset;
		switch (alignment) {
		case Alignment.Center: 
			offset = new Vector2(-textSize.x/2, -textSize.y/2);
			break;
		case Alignment.Right:
			offset = new Vector2(-textSize.x, -textSize.y/2);
			break;
		default:
			offset = Vector2.zero;
			break;
		}
		if (border) {
			if(borderColor == new Color()) borderColor = Color.black;
			DrawOutline (new Rect (Screen.width * 0.3f + offset.x + x, Screen.height * 0.5f + offset.y + y, 0, 0), text, style, borderColor);
			DrawOutline(new Rect( Screen.width * 0.7f + offset.x + x, Screen.height * 0.5f + offset.y + y, 0, 0), text, style, borderColor);
		} else {
			GUI.Label (new Rect (Screen.width * 0.3f + offset.x + x, Screen.height * 0.5f + offset.y + y, 0, 0), text, style);
			GUI.Label (new Rect (Screen.width * 0.7f + offset.x + x, Screen.height * 0.5f + offset.y + y, 0, 0), text, style);
		}
	}
	
	private static void DrawOutline(Rect position, string text, GUIStyle style, Color outColor){
		var backupStyle = style;
		var oldColor = style.normal.textColor;
		style.normal.textColor = outColor;
		position.x--;
		GUI.Label(position, text, style);
		position.x +=2;
		GUI.Label(position, text, style);
		position.x--;
		position.y--;
		GUI.Label(position, text, style);
		position.y +=2;
		GUI.Label(position, text, style);
		position.y--;
		style.normal.textColor = oldColor;
		GUI.Label(position, text, style);
		style = backupStyle;   
	}



}
