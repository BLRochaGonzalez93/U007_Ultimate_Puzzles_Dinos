using System;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Settings
{
    public static class SettingsService
    {
        private const string MusicVolumeKey = "settings.audio.musicVolume";
        private const string SfxVolumeKey = "settings.audio.sfxVolume";
        private const string VibrationEnabledKey = "settings.feedback.vibrationEnabled";

        public const float DefaultMusicVolume = 0.8f;
        public const float DefaultSfxVolume = 0.9f;
        public const bool DefaultVibrationEnabled = true;

        public static event Action Changed;

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            set
            {
                PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
                SaveAndNotify();
            }
        }

        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);
            set
            {
                PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
                SaveAndNotify();
            }
        }

        public static bool VibrationEnabled
        {
            get => PlayerPrefs.GetInt(VibrationEnabledKey, DefaultVibrationEnabled ? 1 : 0) == 1;
            set
            {
                PlayerPrefs.SetInt(VibrationEnabledKey, value ? 1 : 0);
                SaveAndNotify();
            }
        }

        public static void ResetToDefaults()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, DefaultMusicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, DefaultSfxVolume);
            PlayerPrefs.SetInt(VibrationEnabledKey, DefaultVibrationEnabled ? 1 : 0);
            SaveAndNotify();
        }

        private static void SaveAndNotify()
        {
            PlayerPrefs.Save();
            Changed?.Invoke();
        }
    }
}
