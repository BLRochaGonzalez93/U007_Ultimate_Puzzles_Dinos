#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Monetization;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class GoogleMobileAdsIntegrationTools
    {
        private const string IntegrationSymbol = "GOOGLE_MOBILE_ADS";
        private const string ConfigPath =
            "Assets/_Project/Resources/Monetization/AdMobConfig.asset";

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Monetization/" +
            "Enable Google Mobile Ads Integration")]
        public static void EnableIntegration()
        {
            if (!IsSdkPresent())
            {
                EditorUtility.DisplayDialog(
                    "Google Mobile Ads SDK not detected",
                    "Import the official Google Mobile Ads Unity Plugin " +
                    "first. Then run this command again.",
                    "OK");
                return;
            }

            AddDefineSymbol();
            Debug.Log(
                "[AdMob] GOOGLE_MOBILE_ADS enabled for Android.");

            EditorUtility.DisplayDialog(
                "Google Mobile Ads",
                "Integration symbol enabled. Unity will recompile.\n\n" +
                "Next: open Assets > Google Mobile Ads > Settings and " +
                "configure the Android AdMob App ID.",
                "OK");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Monetization/" +
            "Disable Google Mobile Ads Integration")]
        public static void DisableIntegration()
        {
            RemoveDefineSymbol();

            Debug.Log(
                "[AdMob] GOOGLE_MOBILE_ADS disabled for Android.");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Monetization/" +
            "Open AdMob Config")]
        public static void OpenConfig()
        {
            AdMobConfig config =
                AssetDatabase.LoadAssetAtPath<AdMobConfig>(ConfigPath);

            if (config == null)
            {
                Debug.LogError(
                    "[AdMob] Missing AdMobConfig.asset.");
                return;
            }

            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Monetization/" +
            "Validate Google Mobile Ads")]
        public static void Validate()
        {
            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();

            AdMobConfig config =
                AssetDatabase.LoadAssetAtPath<AdMobConfig>(ConfigPath);

            if (config == null)
            {
                errors.Add("Missing AdMobConfig.asset.");
            }
            else
            {
                if (!config.HasRequiredAndroidIds)
                {
                    errors.Add(
                        "AdMobConfig does not contain all required " +
                        "Android IDs.");
                }

                if (!config.UseGoogleTestAds &&
                    config.AndroidAppId.StartsWith(
                        "ca-app-pub-3940256099942544",
                        StringComparison.Ordinal))
                {
                    errors.Add(
                        "Production mode is using a Google sample App ID.");
                }

                if (config.UseGoogleTestAds)
                {
                    warnings.Add(
                        "Google test ads are enabled. Correct for " +
                        "development; disable before production.");
                }

                if (!config.TreatAsChildDirected)
                {
                    warnings.Add(
                        "Child-directed treatment is disabled.");
                }

                if (!config.MaxAdContentRatingGeneral)
                {
                    warnings.Add(
                        "Maximum ad content rating G is disabled.");
                }
            }

            bool sdkPresent = IsSdkPresent();
            bool symbolEnabled = HasDefineSymbol();

            if (!sdkPresent)
            {
                errors.Add(
                    "Google Mobile Ads Unity Plugin is not imported.");
            }

            if (sdkPresent && !symbolEnabled)
            {
                errors.Add(
                    "SDK detected but GOOGLE_MOBILE_ADS is not enabled.");
            }

            if (!sdkPresent && symbolEnabled)
            {
                errors.Add(
                    "GOOGLE_MOBILE_ADS is enabled but SDK is missing.");
            }

            string report =
                "Google Mobile Ads / UMP validation\n\n" +
                "Errors: " + errors.Count + "\n" +
                "Warnings: " + warnings.Count;

            if (errors.Count > 0)
            {
                report += "\n\nERRORS\n- " +
                    string.Join("\n- ", errors);
            }

            if (warnings.Count > 0)
            {
                report += "\n\nWARNINGS\n- " +
                    string.Join("\n- ", warnings);
            }

            if (errors.Count > 0)
            {
                Debug.LogError("[AdMob] " + report);
            }
            else if (warnings.Count > 0)
            {
                Debug.LogWarning("[AdMob] " + report);
            }
            else
            {
                Debug.Log("[AdMob] " + report);
            }

            EditorUtility.DisplayDialog(
                "Google Mobile Ads Validation",
                report,
                "OK");
        }

        private static bool IsSdkPresent()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Any(
                    assembly =>
                        assembly.GetType(
                            "GoogleMobileAds.Api.MobileAds",
                            false) != null);
        }

        private static bool HasDefineSymbol()
        {
            string current =
                PlayerSettings.GetScriptingDefineSymbols(
                    NamedBuildTarget.Android);

            return current
                .Split(';')
                .Any(
                    item => string.Equals(
                        item.Trim(),
                        IntegrationSymbol,
                        StringComparison.Ordinal));
        }

        private static void AddDefineSymbol()
        {
            HashSet<string> symbols =
                GetSymbols();

            if (!symbols.Add(IntegrationSymbol))
            {
                return;
            }

            SaveSymbols(symbols);
        }

        private static void RemoveDefineSymbol()
        {
            HashSet<string> symbols =
                GetSymbols();

            symbols.Remove(IntegrationSymbol);
            SaveSymbols(symbols);
        }

        private static HashSet<string> GetSymbols()
        {
            string current =
                PlayerSettings.GetScriptingDefineSymbols(
                    NamedBuildTarget.Android);

            return new HashSet<string>(
                current
                    .Split(';')
                    .Select(item => item.Trim())
                    .Where(item => !string.IsNullOrEmpty(item)),
                StringComparer.Ordinal);
        }

        private static void SaveSymbols(
            IEnumerable<string> symbols)
        {
            PlayerSettings.SetScriptingDefineSymbols(
                NamedBuildTarget.Android,
                string.Join(";", symbols.OrderBy(item => item)));
        }
    }
}
#endif
