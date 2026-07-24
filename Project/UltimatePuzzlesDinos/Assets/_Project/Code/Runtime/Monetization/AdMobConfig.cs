using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    [CreateAssetMenu(
        fileName = "AdMobConfig",
        menuName = "VRM Games/Ultimate Puzzles Dinos/AdMob Config")]
    public sealed class AdMobConfig : ScriptableObject
    {
        [Header("Environment")]
        [SerializeField] private bool useGoogleTestAds = true;

        [Header("Android - Google sample/test IDs")]
        [SerializeField] private string testAndroidAppId =
            "ca-app-pub-3940256099942544~3347511713";
        [SerializeField] private string testAndroidRewardedId =
            "ca-app-pub-3940256099942544/5224354917";
        [SerializeField] private string testAndroidInterstitialId =
            "ca-app-pub-3940256099942544/1033173712";

        [Header("Android - Production IDs")]
        [SerializeField] private string productionAndroidAppId = "";
        [SerializeField] private string productionAndroidRewardedId = "";
        [SerializeField] private string productionAndroidInterstitialId = "";

        [Header("Child / family treatment")]
        [SerializeField] private bool treatAsChildDirected = true;
        [SerializeField] private bool tagUmpAsUnderAgeOfConsent = true;
        [SerializeField] private bool maxAdContentRatingGeneral = true;

        [Header("Diagnostics")]
        [SerializeField] private bool verboseLogging = true;

        public bool UseGoogleTestAds => useGoogleTestAds;
        public bool TreatAsChildDirected => treatAsChildDirected;
        public bool TagUmpAsUnderAgeOfConsent => tagUmpAsUnderAgeOfConsent;
        public bool MaxAdContentRatingGeneral => maxAdContentRatingGeneral;
        public bool VerboseLogging => verboseLogging;

        public string AndroidAppId =>
            useGoogleTestAds ? testAndroidAppId : productionAndroidAppId;

        public string AndroidRewardedId =>
            useGoogleTestAds ? testAndroidRewardedId : productionAndroidRewardedId;

        public string AndroidInterstitialId =>
            useGoogleTestAds ? testAndroidInterstitialId : productionAndroidInterstitialId;

        public bool HasRequiredAndroidIds =>
            !string.IsNullOrWhiteSpace(AndroidAppId) &&
            !string.IsNullOrWhiteSpace(AndroidRewardedId) &&
            !string.IsNullOrWhiteSpace(AndroidInterstitialId);
    }
}
