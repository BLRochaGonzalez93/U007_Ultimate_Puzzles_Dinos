#if UNITY_EDITOR
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public sealed class AndroidPublishingConfig : ScriptableObject
    {
        [SerializeField] private string companyName = "VRM Games";
        [SerializeField] private string freeProductName = "Ultimate Puzzles Dinos";
        [SerializeField] private string premiumProductName = "Ultimate Puzzles Dinos Premium";
        [SerializeField] private string bundleVersion = "1.0.0";
        [SerializeField, Min(1)] private int freeBundleVersionCode = 1;
        [SerializeField, Min(1)] private int premiumBundleVersionCode = 1;

        [Header("Branding assets")]
        [SerializeField] private Texture2D applicationIcon;
        [SerializeField] private Color splashBackground =
            new Color(0.055f, 0.055f, 0.055f, 1f);
        [SerializeField] private bool showUnityLogo;

        [Header("Google Play metadata")]
        [SerializeField, TextArea(2, 4)]
        private string privacyPolicyUrl = "";
        [SerializeField] private string supportEmail = "";
        [SerializeField] private string websiteUrl = "";

        public string CompanyName => companyName;
        public string FreeProductName => freeProductName;
        public string PremiumProductName => premiumProductName;
        public string BundleVersion => bundleVersion;
        public int FreeBundleVersionCode => freeBundleVersionCode;
        public int PremiumBundleVersionCode => premiumBundleVersionCode;
        public Texture2D ApplicationIcon => applicationIcon;
        public Color SplashBackground => splashBackground;
        public bool ShowUnityLogo => showUnityLogo;
        public string PrivacyPolicyUrl => privacyPolicyUrl;
        public string SupportEmail => supportEmail;
        public string WebsiteUrl => websiteUrl;
    }
}
#endif
