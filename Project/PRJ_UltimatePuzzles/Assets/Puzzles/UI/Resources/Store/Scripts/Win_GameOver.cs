using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Win_GameOver : MonoBehaviour
{
	
	void Awake()
	{

	}

	void Start()
	{
		UIRoot.Show(gameObject);
	}

	public void ButtonSound()
	{
		GameObject.Find("BaseButton").GetComponent<AudioSource>().Play();
	}
	public void ButtonCloseSound()
	{
		GameObject.Find("ButtonClose").GetComponent<AudioSource>().Play();
	}
	public void BackMainMenu()
	{
		if (GameObject.Find("Board").transform.childCount > 0)
		{
			Destroy(GameObject.Find("Board").transform.GetChild(0).gameObject);
		}
		GameObject.Find("LevelManager").GetComponent<LvlManager>().AddLimit(Game.gameID);
		UIRoot.CloseAll();
		UIRoot.Load(WindowName.Win_MainMenu);
	}

	public void Replay()
	{
		GameObject.Find("LevelManager").GetComponent<LvlManager>().AddLimit(Game.gameID);
		UIRoot.Close(gameObject);
		EventDispatcher.SendEvent(EventName.BoardStartGame, Game.image);
	}

	public void NextPic()
	{
		GameObject.Find("LevelManager").GetComponent<LvlManager>().AddLimit(Game.gameID);
		UIRoot.Close(gameObject);
		Game.image = Content.currentGame.GetNextPic(Game.image);
		EventDispatcher.SendEvent(EventName.BoardStartGame, Game.image);
	}
	
	public void BackGameMenu()
	{
		GameObject.Find("LevelManager").GetComponent<LvlManager>().AddLimit(Game.gameID);
		EventDispatcher.SendEvent(EventName.GameSelect, Game.gameID);
	}
}
