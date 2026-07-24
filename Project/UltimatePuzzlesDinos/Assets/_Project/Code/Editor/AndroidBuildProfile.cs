#if UNITY_EDITOR
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Configuration;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public enum AndroidBuildFlavor
    {
        FreeDevelopment,
        FreeRelease,
        PremiumDevelopment,
        PremiumRelease
    }

    public sealed class AndroidBuildProfile : ScriptableObject
    {
        [SerializeField] private AndroidBuildFlavor flavor;
        [SerializeField] private EditionConfig editionConfig;
        [SerializeField] private string productName = "Ultimate Puzzles Dinos";
        [SerializeField] private string applicationIdentifier = "com.vrmgames.ultimatepuzzlesdinos";
        [SerializeField] private string bundleVersion = "1.0.0";
        [SerializeField, Min(1)] private int bundleVersionCode = 1;
        [SerializeField] private bool developmentBuild;
        [SerializeField] private bool allowDebugging;
        [SerializeField] private bool buildAppBundle = true;
        [SerializeField] private string outputFolder = "Builds/Android";

        public AndroidBuildFlavor Flavor => flavor;
        public EditionConfig EditionConfig => editionConfig;
        public string ProductName => productName;
        public string ApplicationIdentifier => applicationIdentifier;
        public string BundleVersion => bundleVersion;
        public int BundleVersionCode => bundleVersionCode;
        public bool DevelopmentBuild => developmentBuild;
        public bool AllowDebugging => allowDebugging;
        public bool BuildAppBundle => buildAppBundle;
        public string OutputFolder => outputFolder;
    }
}
#endif
