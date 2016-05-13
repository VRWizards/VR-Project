using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class menuScript : MonoBehaviour {
	public Canvas quitMenu;
	public Button singleText;
	public Button multiText;
	public Button optionText;
	public Button quitText;
	// Use this for initialization
	void Start () {
		quitMenu = quitMenu.GetComponent<Canvas> ();
		singleText = singleText.GetComponent<Button> ();
		multiText = multiText.GetComponent <Button> ();
		optionText = optionText.GetComponent <Button> ();
		quitText = quitText.GetComponent<Button> ();
		quitMenu.enabled = false;

	}
	
	public void ExitPress() {
		quitMenu.enabled = true;
		singleText.enabled = false;
		quitText.enabled = false;
		multiText.enabled = false;
		optionText.enabled = false;
	}

	public void NoPress(){
		quitMenu.enabled = false;
		singleText.enabled = true;
		quitText.enabled = true;
		multiText.enabled = true;
		optionText.enabled = true;
	}
	public void StartSingle()
	{
		SceneManager.LoadScene ("ShieldTest");
	}
	public void QuitGame()
	{
		Application.Quit ();
	}
	public void StartMulti()
	{
		SceneManager.LoadScene ("Flytest");
	}
}
