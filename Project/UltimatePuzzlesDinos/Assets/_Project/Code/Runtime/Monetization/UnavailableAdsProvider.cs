using System;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public sealed class UnavailableAdsProvider : IAdsProvider
    {
        public bool IsInitialized => false;

        public void Initialize(Action<bool> completed = null)
        {
            Debug.LogError(
                "[Ads] Google Mobile Ads is unavailable in this build. " +
                "No simulated reward will be granted.");
            completed?.Invoke(false);
        }

        public bool IsRewardedAvailable(AdPlacement placement) => false;
        public bool IsInterstitialAvailable(AdPlacement placement) => false;

        public void ShowRewarded(AdPlacement placement, Action<bool> completed)
        {
            completed?.Invoke(false);
        }

        public void ShowInterstitial(
            AdPlacement placement,
            Action completed = null)
        {
            completed?.Invoke();
        }
    }
}
