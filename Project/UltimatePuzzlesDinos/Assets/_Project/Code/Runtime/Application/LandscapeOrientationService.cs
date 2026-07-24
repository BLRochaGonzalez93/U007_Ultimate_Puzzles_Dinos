using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Application
{
    public static class LandscapeOrientationService
    {
        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Apply()
        {
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;

            Debug.Log(
                "[Display] Landscape-only auto rotation enabled. " +
                "Landscape Left/Right are allowed.");
        }
    }
}
