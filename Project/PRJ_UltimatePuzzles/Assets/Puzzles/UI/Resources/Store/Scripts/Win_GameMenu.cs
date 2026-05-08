using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Win_GameMenu : MonoBehaviour
{
    public static int resID;    // ID current size (0 = 2x2, 1=3x3 ...)

    public Transform rootImages;
    public UIImageItem prefImage;
    public Text labelTitle;
    public Text labelSize;
    public int resolutions;
    public int maxLvl;

    void Awake()
    {
        labelSize.text = (Game.piecesY * Game.piecesX).ToString();


        // subscribe to image selection Event
        EventDispatcher.Add(EventName.GameSelectImage, GameSelectImage);
    }

    void Start()
    {
        UIRoot.Show(gameObject);
    }

    // set game data when loading windows
    void SetData(object[] args)
    {


        GameType game = (GameType)args[0];


        int resolutions = GameObject.Find("LevelManager").GetComponent<LvlManager>().resolutions;


        maxLvl = GameObject.Find("LevelManager").GetComponent<LvlManager>().ReadLvl(Game.gameID);
        labelTitle.text = game.title;

        for (int i = 0; i < game.images.Count; i++)
        {
            UIImageItem item = Lib.AddObject<UIImageItem>(prefImage, rootImages);
            /* if (i <= game.images.Count / 3)
             {
                 if (game.ID == 3)
                     item.resolution =resolutions;
                 else
                 {
                     item.resolution = resolutions + 1;
                 }
             }
             else if (i > (game.images.Count / 3) && i <= ((game.images.Count / 3) * 2))
             {
                 if (game.ID == 3)
                     item.resolution = resolutions+1;
                 else
                 {
                     item.resolution = resolutions+2 ;
                 }
             }
             else
             {
                 if (game.ID == 3)
                     item.resolution = resolutions+2;
                 else
                 {
                     item.resolution = resolutions+3;
                 }
             }

             if (i <= maxLvl)
             {
               //  item.IsLocked = false;
             }
             else if (i > maxLvl)
             {
               //  item.IsLocked = true;
              //   item.GetComponentInChildren<Button>().enabled = false;
             //    item.image.color = Color.red;
             }*/
            item.SetData(game.images[i]);
        }

    }

    // button MainMenu

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

        if (GameObject.Find("Board").transform.childCount>0)
        {
        Destroy(GameObject.Find("Board").transform.GetChild(0).gameObject);
        }
        UIRoot.Close(gameObject);
        UIRoot.Load(WindowName.Win_MainMenu);
    }

    // button Prev
    public void PrevSize()
    {
        Game.piecesX--;
        Game.piecesY--;
        if (Game.piecesX < 3)
            Game.piecesX = 3;
        if (Game.piecesY < 3)
            Game.piecesY = 3;

        labelSize.text = (Game.piecesY * Game.piecesX).ToString();
    }

    // button Next
    public void NextSize()
    {
        Game.piecesX++;
        Game.piecesY++;
        if (Game.piecesX > 8)
            Game.piecesX = 8;
        if (Game.piecesY > 8)
            Game.piecesY = 8;

        labelSize.text = (Game.piecesY * Game.piecesX).ToString();
    }




    // Event selection of pictures

    void GameSelectImage(object[] args)
    {
        Sprite image = (Sprite)args[0];

        // load the game with the selected picture
        EventDispatcher.SendEvent(EventName.BoardStartGame, image);
    }

}
