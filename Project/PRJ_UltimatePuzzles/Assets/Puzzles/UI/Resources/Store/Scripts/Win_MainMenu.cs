using UnityEngine;
using System.Collections.Generic;

public class Win_MainMenu : MonoBehaviour
{
	void Start()
	{
		UIRoot.Show(gameObject);
	}

	// The game selection buttons
	public void SelectGame(int value)
	{
		EventDispatcher.SendEvent(EventName.GameSelect, value);	// value = ID game
		UIRoot.Close(gameObject);
	}

	public void SelectDifficulty(int add)
    {
	 GameObject.Find("LevelManager").GetComponent<LvlManager>().DifficultyValues(add);

	}

	public void SettingsButton()
	{
		GameObject.Find("SettingsButton").SetActive(true);
	}
	public void GoToSettings()
	{
		GameObject.Find("UI").transform.GetChild(2).gameObject.SetActive(true);
	}
	public void ButtonSound()
	{
		GameObject.Find("BaseButton").GetComponent<AudioSource>().Play();
	}
	public void ButtonCloseSound()
	{
		GameObject.Find("ButtonClose").GetComponent<AudioSource>().Play();
	}

	public void ExitTheGame()
	{
		Application.Quit();
	}
}
