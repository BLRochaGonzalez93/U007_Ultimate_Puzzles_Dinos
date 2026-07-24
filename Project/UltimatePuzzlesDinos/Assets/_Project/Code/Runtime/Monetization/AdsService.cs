using System;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Configuration;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public static class AdsService
    {
        private const string PolicyResourcePath = "Monetization/AdsPolicy";
        private const string CompletionCountKey = "upd.ads.completed_since_interstitial";
        private const string InterstitialShownKey = "upd.ads.interstitial_shown";
        private const string LastInterstitialTimeKey = "upd.ads.last_interstitial_utc";

        private static EditionConfig editionConfig;
        private static AdsPolicy policy;
        private static IAdsProvider provider;
        private static bool initialized;

        public static bool AdsEnabled =>
            initialized && editionConfig != null &&
            editionConfig.AdsEnabled && !editionConfig.AllContentUnlocked;

        public static bool UsingDevelopmentMock =>
            provider is MockAdsProvider;

        public static bool RewardedUnlockAvailable =>
            AdsEnabled && editionConfig.RewardedUnlocksEnabled &&
            policy != null && policy.RewardedLevelUnlockEnabled &&
            provider != null &&
            provider.IsRewardedAvailable(AdPlacement.LevelUnlockReward);

        public static void Initialize(
            EditionConfig config,
            IAdsProvider customProvider = null)
        {
            editionConfig = config;
            policy = Resources.Load<AdsPolicy>(PolicyResourcePath);
            provider = customProvider ?? CreateDefaultProvider();
            initialized = true;

            if (!AdsEnabled)
            {
                Debug.Log("[Ads] Disabled for the active edition.");
                return;
            }

            provider.Initialize(success =>
            {
                Debug.Log(success
                    ? "[Ads] Provider ready."
                    : "[Ads] Provider initialization failed.");
            });
        }

        public static void ShowLevelUnlockReward(Action<bool> completed)
        {
            if (!RewardedUnlockAvailable)
            {
                completed?.Invoke(false);
                return;
            }

            provider.ShowRewarded(
                AdPlacement.LevelUnlockReward,
                completed);
        }

        public static void RegisterPuzzleCompletion(
            Action completed = null)
        {
            if (!AdsEnabled || policy == null ||
                !policy.InterstitialAfterPuzzleEnabled)
            {
                completed?.Invoke();
                return;
            }

            int count =
                PlayerPrefs.GetInt(CompletionCountKey, 0) + 1;

            PlayerPrefs.SetInt(CompletionCountKey, count);
            PlayerPrefs.Save();

            if (!ShouldShowInterstitial(count))
            {
                completed?.Invoke();
                return;
            }

            if (provider == null ||
                !provider.IsInterstitialAvailable(
                    AdPlacement.PuzzleCompleted))
            {
                completed?.Invoke();
                return;
            }

            provider.ShowInterstitial(
                AdPlacement.PuzzleCompleted,
                () =>
                {
                    PlayerPrefs.SetInt(
                        CompletionCountKey,
                        0);

                    PlayerPrefs.SetInt(
                        InterstitialShownKey,
                        1);

                    PlayerPrefs.SetString(
                        LastInterstitialTimeKey,
                        DateTime.UtcNow.Ticks.ToString());

                    PlayerPrefs.Save();
                    completed?.Invoke();
                });
        }

        public static bool PrivacyOptionsRequired
        {
            get
            {
#if GOOGLE_MOBILE_ADS
                return AdsEnabled &&
                    GoogleMobileAdsProvider.PrivacyOptionsRequired;
#else
                return false;
#endif
            }
        }

        public static void ShowPrivacyOptions(
            Action<bool> completed = null)
        {
#if GOOGLE_MOBILE_ADS
            if (AdsEnabled)
            {
                GoogleMobileAdsProvider.ShowPrivacyOptions(completed);
                return;
            }
#endif
            completed?.Invoke(false);
        }

        private static IAdsProvider CreateDefaultProvider()
        {
#if GOOGLE_MOBILE_ADS
            return new GoogleMobileAdsProvider();
#elif UNITY_EDITOR
            Debug.LogWarning(
                "[Ads] Google Mobile Ads SDK integration is not enabled. " +
                "Using MockAdsProvider in Editor only.");

            return new MockAdsProvider();
#else
            return new UnavailableAdsProvider();
#endif
        }

        private static bool ShouldShowInterstitial(
            int completionCount)
        {
            if (policy.SkipFirstInterstitial &&
                PlayerPrefs.GetInt(
                    InterstitialShownKey,
                    0) == 0 &&
                completionCount <
                    policy.CompletedPuzzlesBetweenInterstitials + 1)
            {
                return false;
            }

            if (completionCount <
                policy.CompletedPuzzlesBetweenInterstitials)
            {
                return false;
            }

            string storedTicks =
                PlayerPrefs.GetString(
                    LastInterstitialTimeKey,
                    string.Empty);

            if (!long.TryParse(
                    storedTicks,
                    out long ticks) ||
                ticks <= 0)
            {
                return true;
            }

            DateTime lastShown =
                new DateTime(
                    ticks,
                    DateTimeKind.Utc);

            return (DateTime.UtcNow - lastShown)
                .TotalSeconds >=
                policy.InterstitialCooldownSeconds;
        }
    }
}
