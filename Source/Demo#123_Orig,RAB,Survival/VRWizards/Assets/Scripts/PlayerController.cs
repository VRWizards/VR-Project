using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//check every frame for player input
//apply this input
public class PlayerController : MonoBehaviour {

	//public float speed;
	public static Text countText;
	public static Text winText;


	//private int count;
	public static int count;

	void Start ()
	{
		countText = GameObject.Find ("CountText").GetComponent<Text> ();
		winText = GameObject.Find ("WinText").GetComponent<Text> ();
		count = 0;
		SetCountText ();
		winText.text = "";
	}



	void OnTriggerEnter(Collider other)
	{
		//Destroy (other.gameObject);
		if (other.gameObject.CompareTag ("Pick Up")) {
			other.gameObject.SetActive(false);
			//count++;
			//SetCountText ();
		}
	}

	public static void SetCountText()
	{
		countText.text = "Count: " + count.ToString ();
		if (count >= 12) {
			winText.text = "You Win!";
		}

	}
}
