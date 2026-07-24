#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class FullscreenAdsDisplayValidator
    {
        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Monetization/" +
            "Validate Fullscreen Display")]
        public static void Validate()
        {
            List<string> errors = new List<string>();

            if (PlayerSettings.defaultInterfaceOrientation !=
                UIOrientation.AutoRotation)
            {
                errors.Add(
                    "Default Orientation debe ser Auto Rotation.");
            }

            if (PlayerSettings.allowedAutorotateToPortrait)
            {
                errors.Add(
                    "Portrait debe estar deshabilitado.");
            }

            if (PlayerSettings.allowedAutorotateToPortraitUpsideDown)
            {
                errors.Add(
                    "Portrait Upside Down debe estar deshabilitado.");
            }

            if (!PlayerSettings.allowedAutorotateToLandscapeLeft)
            {
                errors.Add(
                    "Landscape Left debe estar habilitado.");
            }

            if (!PlayerSettings.allowedAutorotateToLandscapeRight)
            {
                errors.Add(
                    "Landscape Right debe estar habilitado.");
            }

            if (errors.Count == 0)
            {
                Debug.Log(
                    "[Display] Fullscreen ads display configuration " +
                    "is valid: landscape-only Left/Right.");

                EditorUtility.DisplayDialog(
                    "Fullscreen Ads",
                    "Configuración correcta.\n\n" +
                    "Auto Rotation: ON\n" +
                    "Portrait: OFF\n" +
                    "Landscape Left: ON\n" +
                    "Landscape Right: ON",
                    "OK");

                return;
            }

            string report =
                string.Join("\n- ", errors);

            Debug.LogError(
                "[Display] Validation failed:\n- " +
                report);

            EditorUtility.DisplayDialog(
                "Fullscreen Ads",
                "Problemas encontrados:\n\n- " +
                report,
                "OK");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Monetization/" +
            "Apply Landscape-Only Display")]
        public static void ApplyLandscapeOnly()
        {
            PlayerSettings.defaultInterfaceOrientation =
                UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            AssetDatabase.SaveAssets();

            Debug.Log(
                "[Display] Landscape-only display configuration applied.");
        }
    }
}
#endif
