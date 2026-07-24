using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Settings;

namespace VRMGames.UltimatePuzzlesDinos.Haptics
{
    public static class HapticService
    {
        public static void Play(HapticCue cue)
        {
            if (!SettingsService.VibrationEnabled)
            {
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            if (UnityEngine.Application.isMobilePlatform)
            {
                Handheld.Vibrate();
            }
#endif
        }
    }
}
