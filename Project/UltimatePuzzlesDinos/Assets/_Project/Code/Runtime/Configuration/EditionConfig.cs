using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Configuration
{
    [CreateAssetMenu(
        fileName = "EditionConfig",
        menuName = "VRM Games/Ultimate Puzzles Dinos/Edition Configuration")]
    public sealed class EditionConfig : ScriptableObject
    {
        [SerializeField] private GameEdition edition = GameEdition.Free;
        [SerializeField] private bool adsEnabled = true;
        [SerializeField] private bool rewardedUnlocksEnabled = true;
        [SerializeField] private bool allContentUnlocked;
        [SerializeField, Min(0)] private int initiallyUnlockedPuzzleCount = 3;

        public GameEdition Edition => edition;
        public bool AdsEnabled => adsEnabled;
        public bool RewardedUnlocksEnabled => rewardedUnlocksEnabled;
        public bool AllContentUnlocked => allContentUnlocked;
        public int InitiallyUnlockedPuzzleCount => initiallyUnlockedPuzzleCount;

#if UNITY_EDITOR
        public void Configure(
            GameEdition targetEdition,
            bool enableAds,
            bool enableRewardedUnlocks,
            bool unlockAllContent,
            int unlockedPuzzleCount)
        {
            edition = targetEdition;
            adsEnabled = enableAds;
            rewardedUnlocksEnabled = enableRewardedUnlocks;
            allContentUnlocked = unlockAllContent;
            initiallyUnlockedPuzzleCount = Mathf.Max(0, unlockedPuzzleCount);
        }
#endif
    }
}
