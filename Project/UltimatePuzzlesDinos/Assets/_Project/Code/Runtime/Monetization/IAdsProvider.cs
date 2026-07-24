using System;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public interface IAdsProvider
    {
        bool IsInitialized { get; }
        bool IsRewardedAvailable(AdPlacement placement);
        bool IsInterstitialAvailable(AdPlacement placement);
        void Initialize(Action<bool> completed = null);
        void ShowRewarded(AdPlacement placement, Action<bool> completed);
        void ShowInterstitial(AdPlacement placement, Action completed = null);
    }
}
