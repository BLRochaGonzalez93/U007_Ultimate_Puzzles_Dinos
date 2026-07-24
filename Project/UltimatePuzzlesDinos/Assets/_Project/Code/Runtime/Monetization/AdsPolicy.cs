using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    [CreateAssetMenu(
        fileName = "AdsPolicy",
        menuName = "VRM Games/Ultimate Puzzles Dinos/Ads Policy")]
    public sealed class AdsPolicy : ScriptableObject
    {
        [Header("Rewarded")]
        [SerializeField] private bool rewardedLevelUnlockEnabled = true;

        [Header("Interstitial")]
        [SerializeField] private bool interstitialAfterPuzzleEnabled = true;
        [SerializeField, Min(1)] private int completedPuzzlesBetweenInterstitials = 3;
        [SerializeField, Min(0f)] private float interstitialCooldownSeconds = 120f;
        [SerializeField] private bool skipFirstInterstitial = true;

        public bool RewardedLevelUnlockEnabled => rewardedLevelUnlockEnabled;
        public bool InterstitialAfterPuzzleEnabled => interstitialAfterPuzzleEnabled;
        public int CompletedPuzzlesBetweenInterstitials =>
            Mathf.Max(1, completedPuzzlesBetweenInterstitials);
        public float InterstitialCooldownSeconds =>
            Mathf.Max(0f, interstitialCooldownSeconds);
        public bool SkipFirstInterstitial => skipFirstInterstitial;
    }
}
