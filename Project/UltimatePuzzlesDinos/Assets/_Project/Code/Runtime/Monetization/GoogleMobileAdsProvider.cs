#if GOOGLE_MOBILE_ADS
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public sealed class GoogleMobileAdsProvider : IAdsProvider
    {
        private const string ConfigResourcePath =
            "Monetization/AdMobConfig";

        private AdMobConfig config;
        private RewardedAd rewardedAd;
        private InterstitialAd interstitialAd;
        private bool initializing;
        private bool initialized;
        private bool fullscreenRequestInProgress;

        public bool IsInitialized => initialized;

        public void Initialize(Action<bool> completed = null)
        {
            if (initialized)
            {
                completed?.Invoke(true);
                return;
            }

            if (initializing)
            {
                completed?.Invoke(false);
                return;
            }

            config = Resources.Load<AdMobConfig>(
                ConfigResourcePath);

            if (config == null ||
                !config.HasRequiredAndroidIds)
            {
                Debug.LogError(
                    "[AdMob] AdMobConfig is missing or does not " +
                    "contain the required Android IDs.");

                completed?.Invoke(false);
                return;
            }

            initializing = true;

            ApplyGlobalRequestConfiguration();

            ConsentRequestParameters consentParameters =
                new ConsentRequestParameters
                {
                    TagForUnderAgeOfConsent =
                        config.TagUmpAsUnderAgeOfConsent
                };

            ConsentInformation.Update(
                consentParameters,
                updateError =>
                {
                    if (updateError != null)
                    {
                        Debug.LogWarning(
                            "[UMP] Consent information update failed: " +
                            updateError.Message);
                    }

                    ConsentForm.LoadAndShowConsentFormIfRequired(
                        formError =>
                        {
                            if (formError != null)
                            {
                                Debug.LogWarning(
                                    "[UMP] Consent form failed: " +
                                    formError.Message);
                            }

                            if (!ConsentInformation.CanRequestAds())
                            {
                                initializing = false;

                                Debug.LogWarning(
                                    "[UMP] Ads cannot be requested yet.");

                                completed?.Invoke(false);
                                return;
                            }

                            InitializeMobileAds(completed);
                        });
                });
        }

        public bool IsRewardedAvailable(
            AdPlacement placement)
        {
            return
                initialized &&
                !fullscreenRequestInProgress &&
                rewardedAd != null &&
                rewardedAd.CanShowAd();
        }

        public bool IsInterstitialAvailable(
            AdPlacement placement)
        {
            return
                initialized &&
                !fullscreenRequestInProgress &&
                interstitialAd != null &&
                interstitialAd.CanShowAd();
        }

        public void ShowRewarded(
            AdPlacement placement,
            Action<bool> completed)
        {
            if (!IsRewardedAvailable(placement))
            {
                completed?.Invoke(false);
                LoadRewarded();
                return;
            }

            fullscreenRequestInProgress = true;

            FullscreenAdDisplayGuard.WaitUntilReady(
                ready =>
                {
                    if (!ready)
                    {
                        fullscreenRequestInProgress = false;

                        Debug.LogWarning(
                            "[AdMob] Rewarded display cancelled because " +
                            "the screen was not stable in landscape.");

                        completed?.Invoke(false);
                        return;
                    }

                    ShowRewardedInternal(completed);
                });
        }

        public void ShowInterstitial(
            AdPlacement placement,
            Action completed = null)
        {
            if (!IsInterstitialAvailable(placement))
            {
                completed?.Invoke();
                LoadInterstitial();
                return;
            }

            fullscreenRequestInProgress = true;

            FullscreenAdDisplayGuard.WaitUntilReady(
                ready =>
                {
                    if (!ready)
                    {
                        fullscreenRequestInProgress = false;

                        Debug.LogWarning(
                            "[AdMob] Interstitial display cancelled " +
                            "because the screen was not stable in " +
                            "landscape.");

                        completed?.Invoke();
                        return;
                    }

                    ShowInterstitialInternal(completed);
                });
        }

        public static bool PrivacyOptionsRequired =>
            ConsentInformation.PrivacyOptionsRequirementStatus ==
            PrivacyOptionsRequirementStatus.Required;

        public static void ShowPrivacyOptions(
            Action<bool> completed = null)
        {
            ConsentForm.ShowPrivacyOptionsForm(
                error =>
                {
                    if (error != null)
                    {
                        Debug.LogWarning(
                            "[UMP] Privacy options failed: " +
                            error.Message);

                        completed?.Invoke(false);
                        return;
                    }

                    completed?.Invoke(true);
                });
        }

        private void ShowRewardedInternal(
            Action<bool> completed)
        {
            if (rewardedAd == null ||
                !rewardedAd.CanShowAd())
            {
                fullscreenRequestInProgress = false;
                completed?.Invoke(false);
                LoadRewarded();
                return;
            }

            RewardedAd ad = rewardedAd;
            rewardedAd = null;
            bool rewardEarned = false;
            bool callbackSent = false;

            void Complete(bool result)
            {
                if (callbackSent)
                {
                    return;
                }

                callbackSent = true;
                fullscreenRequestInProgress = false;
                completed?.Invoke(result);
            }

            ad.OnAdFullScreenContentOpened += () =>
            {
                LogFullscreenState(
                    "[AdMob] Rewarded opened");
            };

            ad.OnAdFullScreenContentClosed += () =>
            {
                ad.Destroy();
                LoadRewarded();
                Complete(rewardEarned);
            };

            ad.OnAdFullScreenContentFailed += error =>
            {
                Debug.LogWarning(
                    "[AdMob] Rewarded failed to show: " +
                    error.GetMessage());

                ad.Destroy();
                LoadRewarded();
                Complete(false);
            };

            ad.Show(
                reward =>
                {
                    rewardEarned = true;

                    if (config.VerboseLogging)
                    {
                        Debug.Log(
                            "[AdMob] Reward earned. Type=" +
                            reward.Type +
                            ", Amount=" +
                            reward.Amount);
                    }
                });
        }

        private void ShowInterstitialInternal(
            Action completed)
        {
            if (interstitialAd == null ||
                !interstitialAd.CanShowAd())
            {
                fullscreenRequestInProgress = false;
                completed?.Invoke();
                LoadInterstitial();
                return;
            }

            InterstitialAd ad = interstitialAd;
            interstitialAd = null;
            bool callbackSent = false;

            void Complete()
            {
                if (callbackSent)
                {
                    return;
                }

                callbackSent = true;
                fullscreenRequestInProgress = false;
                completed?.Invoke();
            }

            ad.OnAdFullScreenContentOpened += () =>
            {
                LogFullscreenState(
                    "[AdMob] Interstitial opened");
            };

            ad.OnAdFullScreenContentClosed += () =>
            {
                ad.Destroy();
                LoadInterstitial();
                Complete();
            };

            ad.OnAdFullScreenContentFailed += error =>
            {
                Debug.LogWarning(
                    "[AdMob] Interstitial failed to show: " +
                    error.GetMessage());

                ad.Destroy();
                LoadInterstitial();
                Complete();
            };

            ad.Show();
        }

        private void InitializeMobileAds(
            Action<bool> completed)
        {
            MobileAds.Initialize(
                status =>
                {
                    initialized = true;
                    initializing = false;

                    LoadRewarded();
                    LoadInterstitial();

                    Debug.Log(
                        "[AdMob] Google Mobile Ads initialized.");

                    completed?.Invoke(true);
                });
        }

        private void ApplyGlobalRequestConfiguration()
        {
            RequestConfiguration requestConfiguration =
                new RequestConfiguration();

            requestConfiguration.AgeRestrictedTreatment =
                config.TreatAsChildDirected
                    ? AgeRestrictedTreatment.Child
                    : AgeRestrictedTreatment.Unspecified;

            if (config.MaxAdContentRatingGeneral)
            {
                requestConfiguration.MaxAdContentRating =
                    MaxAdContentRating.G;
            }

            MobileAds.SetRequestConfiguration(
                requestConfiguration);
        }

        private void LoadRewarded()
        {
            DestroyRewarded();

            RewardedAd.Load(
                config.AndroidRewardedId,
                new AdRequest(),
                (ad, error) =>
                {
                    if (error != null ||
                        ad == null)
                    {
                        Debug.LogWarning(
                            "[AdMob] Rewarded load failed: " +
                            (error != null
                                ? error.GetMessage()
                                : "null ad"));

                        return;
                    }

                    rewardedAd = ad;

                    if (config.VerboseLogging)
                    {
                        Debug.Log(
                            "[AdMob] Rewarded ready.");
                    }
                });
        }

        private void LoadInterstitial()
        {
            DestroyInterstitial();

            InterstitialAd.Load(
                config.AndroidInterstitialId,
                new AdRequest(),
                (ad, error) =>
                {
                    if (error != null ||
                        ad == null)
                    {
                        Debug.LogWarning(
                            "[AdMob] Interstitial load failed: " +
                            (error != null
                                ? error.GetMessage()
                                : "null ad"));

                        return;
                    }

                    interstitialAd = ad;

                    if (config.VerboseLogging)
                    {
                        Debug.Log(
                            "[AdMob] Interstitial ready.");
                    }
                });
        }

        private void DestroyRewarded()
        {
            if (rewardedAd == null)
            {
                return;
            }

            rewardedAd.Destroy();
            rewardedAd = null;
        }

        private void DestroyInterstitial()
        {
            if (interstitialAd == null)
            {
                return;
            }

            interstitialAd.Destroy();
            interstitialAd = null;
        }

        private static void LogFullscreenState(
            string prefix)
        {
            Debug.Log(
                $"{prefix}. " +
                $"Resolution={Screen.width}x{Screen.height}, " +
                $"Orientation={Screen.orientation}, " +
                $"SafeArea={Screen.safeArea}.");
        }
    }
}
#endif
