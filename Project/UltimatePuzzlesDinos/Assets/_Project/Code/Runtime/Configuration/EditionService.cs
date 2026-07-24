using System;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Configuration
{
    public static class EditionService
    {
        private static EditionConfig config;

        public static event Action<EditionConfig> EditionInitialized;

        public static bool IsInitialized => config != null;
        public static EditionConfig Config => config;
        public static GameEdition CurrentEdition =>
            config != null ? config.Edition : GameEdition.Free;
        public static bool IsFree => CurrentEdition == GameEdition.Free;
        public static bool IsPremium => CurrentEdition == GameEdition.Premium;
        public static bool AdsEnabled =>
            config != null && config.AdsEnabled && !IsPremium;
        public static bool RewardedUnlocksEnabled =>
            AdsEnabled &&
            config.RewardedUnlocksEnabled &&
            !config.AllContentUnlocked;
        public static bool AllContentUnlocked =>
            config != null && (config.AllContentUnlocked || IsPremium);
        public static int InitiallyUnlockedPuzzleCount =>
            config != null ? config.InitiallyUnlockedPuzzleCount : 3;

        public static void Initialize(EditionConfig editionConfig)
        {
            if (editionConfig == null)
            {
                Debug.LogError(
                    "[EditionService] No EditionConfig was assigned. " +
                    "The application will use safe Free defaults.");
                config = null;
                EditionInitialized?.Invoke(null);
                return;
            }

            config = editionConfig;
            ValidateRuntimeConfiguration();
            EditionInitialized?.Invoke(config);

            Debug.Log(
                $"[EditionService] Initialized {CurrentEdition}. " +
                $"Ads={AdsEnabled}, Rewarded={RewardedUnlocksEnabled}, " +
                $"AllUnlocked={AllContentUnlocked}.");
        }

        private static void ValidateRuntimeConfiguration()
        {
            if (!IsPremium)
            {
                return;
            }

            if (config.AdsEnabled || config.RewardedUnlocksEnabled)
            {
                Debug.LogWarning(
                    "[EditionService] Premium configuration contains ad flags. " +
                    "They are disabled at runtime by edition policy.",
                    config);
            }

            if (!config.AllContentUnlocked)
            {
                Debug.LogWarning(
                    "[EditionService] Premium configuration does not explicitly " +
                    "unlock all content. Premium policy will unlock it at runtime.",
                    config);
            }
        }
    }
}
