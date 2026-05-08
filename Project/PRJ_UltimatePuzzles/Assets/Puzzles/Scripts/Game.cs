using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static int gameID;       // The current game ID
    public static Sprite image;     // current picture
    public static int piecesX;      // the number of pieces on the side
    public static int piecesY;      // the number of pieces on the side

    public Content content;         // object games content
    public Transform rootScene;     // stage root for loading games

    void Awake()
    {
        piecesX = 3;
        piecesY = 3;


        // subscription events
        EventDispatcher.Add(EventName.GameSelect, GameSelect);
        EventDispatcher.Add(EventName.BoardStartGame, BoardStartGame);
        EventDispatcher.Add(EventName.GameOver, GameOver);
    }

    void Start()
    {
        // load MainMenu window
        UIRoot.Load(WindowName.Win_MainMenu);
    }
    /*public void SetPieces(int res)
	{
		piecesX = res;
		piecesY = res;
	}
	*/
    void GameSelect(object[] args)
    {
        // select ID game
        gameID = (int)args[0];
    }

    void BoardStartGame(object[] args)
    {
        // load game
        image = (Sprite)args[0];   // picture
        Debug.Log(args[0]);
        Lib.RemoveObjects(rootScene);
        // load the game from the content list
        Board board = Lib.AddObject<Board>(content.games[gameID].board, rootScene);
        board.SendMessage("SetData", SendMessageOptions.DontRequireReceiver);

        // load the game interface window
        UIRoot.CloseAll();
        UIRoot.Load(WindowName.Win_Board);
    }

    public void HideSettingsButton()
    {
        GameObject.Find("SettingsButton").SetActive(false);
    }
    void GameOver()
    {
        // GameOver show window
        UIRoot.Load(WindowName.Win_GameOver);
    }
    private void FixedUpdate()
    {
        if (GameObject.Find("Win_MainMenu") != null && gameObject.GetComponent<AudioSource>().isPlaying == false)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
        if (GameObject.Find("Win_GameMenu") != null && gameObject.GetComponent<AudioSource>().isPlaying == false)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
        if (GameObject.Find("Win_Board") != null && gameObject.GetComponent<AudioSource>().isPlaying == true)
        {
            gameObject.GetComponent<AudioSource>().Stop();
        }
    }

}
