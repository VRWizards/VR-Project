using UnityEngine;
using System;

[Serializable]
public class SpriteSheet {

	public Texture texture;
	public int columns;
	public int rows;
	public float fps = 23.976f;
	public Vector2 offset = new Vector2(0,0);
	public float width = 1000;
	public float height = 1000;
	public float rotate = 0;

	private float start = float.NaN;

	public void Render() {
		if (IsFinished()) {
			start = Time.time;
		}
		int index = (int) ((Time.time-start) * fps);
		if (index >= columns * rows) {
			start = float.NaN;
			return;
		}
		index = index % (columns * rows);		
		Render (index);
	}

	public void Render(int index) {		
		Vector2 size = new Vector2 (1.0f / columns, 1.0f / rows);		
		int columnIndex = index % columns;
		int rowIndex = index / columns;
		
		Rect textureOffset = new Rect (columnIndex * size.x *1f, 1.0f - size.y - rowIndex * size.y,size.x,size.y);
		float x = Screen.width * 0.3f + offset.x;
		float y = Screen.height / 2 + offset.y;
		if(rotate != 0)	GUIUtility.RotateAroundPivot (rotate, new Vector2(x, y));
		GUI.DrawTextureWithTexCoords (new Rect (x - width/2, y - height/2, width, height), texture, textureOffset);
		if(rotate != 0)	GUIUtility.RotateAroundPivot (-rotate, new Vector2(x, y));
		x = Screen.width * 0.7f + offset.x;
		y = Screen.height / 2 + offset.y;
		if(rotate != 0)	GUIUtility.RotateAroundPivot (rotate, new Vector2(x, y));
		GUI.DrawTextureWithTexCoords (new Rect (x - width/2, y - height/2, width, height), texture, textureOffset);
		if(rotate != 0)	GUIUtility.RotateAroundPivot (-rotate, new Vector2(x, y));
	}

	public bool IsFinished(){
		return float.IsNaN (start);
	}

}
