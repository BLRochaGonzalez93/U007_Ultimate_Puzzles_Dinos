using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LvlManager : MonoBehaviour
{
    public int lvlMaxMosaic, lvlMaxPuzzle, lvlMaxPuzzleLogic;
    public int maxGallery;
    public int resolutions;
    public TMP_Dropdown difficultyDD;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("difficultyIndex"))
        {
            difficultyDD.value = PlayerPrefs.GetInt("difficultyIndex");
            DifficultyValues(difficultyDD.value);
        }
        else
        {
            resolutions = 2;
        }

        if(PlayerPrefs.HasKey("maxPuzzle"))
        {
            lvlMaxPuzzle = PlayerPrefs.GetInt("maxPuzzle");
        }
        else
        {
            lvlMaxPuzzle = 0;
        }
        if (PlayerPrefs.HasKey("maxPuzzleLogic"))
        {
            lvlMaxPuzzleLogic = PlayerPrefs.GetInt("maxPuzzleLogic");
        }
        else
        {
            lvlMaxPuzzleLogic = 0;
        }
        if (PlayerPrefs.HasKey("maxMosaic"))
        {
            lvlMaxMosaic = PlayerPrefs.GetInt("maxMosaic");
        }
        else
        {
            lvlMaxMosaic = 0;
        }

    }
    public void AddLimit(int gameID)
    {
        if (gameID == 1)
        {
            lvlMaxPuzzle++;
            PlayerPrefs.SetInt("maxPuzzle",lvlMaxPuzzle);
        }
        if (gameID == 2)
        {
            lvlMaxPuzzleLogic++;
            PlayerPrefs.SetInt("maxPuzzleLogic", lvlMaxPuzzleLogic);

        }
        if (gameID == 3)
        {
            lvlMaxMosaic++;
            PlayerPrefs.SetInt("maxMosaic", lvlMaxMosaic);

        }
    }

    public void DifficultyValues(int res)
    {
        if (res == 0)
        {
            resolutions = 2;
            PlayerPrefs.SetInt("difficultyIndex", res);
        }
        else if (res == 1)
        {
            resolutions = 3;
            PlayerPrefs.SetInt("difficultyIndex", res);
        }
        else if (res == 2)
        {
            resolutions = 4;
            PlayerPrefs.SetInt("difficultyIndex", res);
        }
    }
    public int MaxGallery(int max)
    {
        max = Mathf.Max(lvlMaxMosaic, lvlMaxPuzzle, lvlMaxPuzzleLogic);
        return max;
    }
    public int ReadLvl(int gameID)
    {
        if (gameID == 1)
        {
            return lvlMaxPuzzle;
        }
        if (gameID == 2)
        {
            return lvlMaxPuzzleLogic;
        }
        if (gameID == 3)
        {
            return lvlMaxMosaic;
        }
        else
        {
            return 0;
        }
    }
}
