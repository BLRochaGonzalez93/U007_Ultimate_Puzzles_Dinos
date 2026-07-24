using System;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public sealed class MockAdsProvider : IAdsProvider
    {
        public bool IsInitialized { get; private set; }

        public void Initialize(Action<bool> completed = null)
        {
            IsInitialized = true;
            Debug.Log("[Ads] Mock provider initialized.");
            completed?.Invoke(true);
        }

        public bool IsRewardedAvailable(AdPlacement placement)
        {
            return IsInitialized;
        }

        public bool IsInterstitialAvailable(AdPlacement placement)
        {
            return IsInitialized;
        }

        public void ShowRewarded(AdPlacement placement, Action<bool> completed)
        {
            Debug.LogWarning(
                $"[Ads] Simulated rewarded ad at {placement}. Reward granted.");
            completed?.Invoke(true);
        }

        public void ShowInterstitial(AdPlacement placement, Action completed = null)
        {
            Debug.LogWarning(
                $"[Ads] Simulated interstitial at {placement}. No visual ad is shown.");
            completed?.Invoke();
        }
    }
}
