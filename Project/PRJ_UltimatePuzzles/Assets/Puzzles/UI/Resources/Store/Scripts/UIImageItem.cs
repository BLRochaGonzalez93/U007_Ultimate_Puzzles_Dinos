using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImageItem : MonoBehaviour
{
	public Image image,locked;
	public bool IsLocked = true, IsCompleted = false;
	public int resolution;

	public void SetData(Sprite sprite)
	{
		image.sprite = sprite;
		if (IsLocked)
		{
			locked.enabled = true;
		}
		else
		{
			locked.enabled = false;
		}
		image.preserveAspect = true;
	}
	public void SelectSound()
	{
		GameObject.Find("LevelSelected").GetComponent<AudioSource>().Play();
	}
	
	public void SetResolution()
    {
		//GameObject.Find("Game").GetComponent<Game>().SetPieces(resolution);
		resolution = Game.piecesX;
    }
	public void SelectImage()
	{
		EventDispatcher.SendEvent(EventName.GameSelectImage, image.sprite);
	}
}
