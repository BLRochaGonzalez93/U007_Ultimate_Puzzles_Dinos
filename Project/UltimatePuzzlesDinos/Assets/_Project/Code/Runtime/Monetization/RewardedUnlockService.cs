using System;
using VRMGames.UltimatePuzzlesDinos.Gameplay;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public static class RewardedUnlockService
    {
        public static bool IsAvailable =>
            AdsService.RewardedUnlockAvailable;

        public static bool IsDevelopmentSimulation =>
            AdsService.UsingDevelopmentMock;

        public static void ShowLevelUnlockReward(
            PuzzleMode mode,
            int levelId,
            Action<bool> completed)
        {
            AdsService.ShowLevelUnlockReward(completed);
        }
    }
}
