using UnityEngine;
using System.Collections;

public class LoadTargetLevel : MonoBehaviour {

	public int level;

	public void Load()
	{
		Application.LoadLevel (level);
	}
}
