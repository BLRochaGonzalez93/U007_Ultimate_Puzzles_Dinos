using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class Win_Board : MonoBehaviour
{
    
    public GameObject imagePannel;
    public Image image;
    void Start()
    {
        UIRoot.Show(gameObject);
    }

    public void BackGameMenu()
    {
        if (GameObject.Find("Board").transform.childCount > 0)
        {
            Destroy(GameObject.Find("Board").transform.GetChild(0).gameObject);
        }
        UIRoot.Close(WindowName.Win_Board);
        EventDispatcher.SendEvent(EventName.GameSelect, Game.gameID);
    }

    public void ButtonSound()
    {
        GameObject.Find("BaseButton").GetComponent<AudioSource>().Play();
    }
    public void ButtonCloseSound()
    {
        GameObject.Find("ButtonClose").GetComponent<AudioSource>().Play();
    }
    public void ManageImage()
    {
        image.sprite = Game.image;
        if (imagePannel.activeInHierarchy)
        {
            imagePannel.SetActive(false);
        }
        else
        {
            imagePannel.SetActive(true);
        }
    }
}
