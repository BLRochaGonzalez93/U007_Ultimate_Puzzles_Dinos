using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider sfxSlider, musicSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
        LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetSfxVolume(); 
        }
        
    }
    public void SetMusicVolume()
    {
        { 
            float volumeMusic = musicSlider.value;
            mixer.SetFloat("Music", Mathf.Log10(volumeMusic) * 20);
            PlayerPrefs.SetFloat("musicVolume", volumeMusic);
        }
    }
    public void SetSfxVolume()
    {
        {
            float volumeSfx = sfxSlider.value;
            mixer.SetFloat("Sfx", Mathf.Log10(volumeSfx) * 20);
            PlayerPrefs.SetFloat("sfxVolume", volumeSfx);
        }
    }
    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetMusicVolume();
        SetSfxVolume();
    }

}
