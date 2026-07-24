using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Performance
{
    [CreateAssetMenu(
        fileName = "MobilePerformanceConfig",
        menuName = "VRM Games/Ultimate Puzzles Dinos/Mobile Performance Config")]
    public sealed class MobilePerformanceConfig : ScriptableObject
    {
        [Header("Active preset")]
        [SerializeField] private MobilePerformanceTier activeTier =
            MobilePerformanceTier.Medium;

        [Header("Low")]
        [SerializeField, Min(15)] private int lowTargetFrameRate = 30;
        [SerializeField, Range(0, 2)] private int lowVSyncCount = 0;
        [SerializeField, Range(0.5f, 1f)] private float lowRenderScale = 0.75f;

        [Header("Medium")]
        [SerializeField, Min(15)] private int mediumTargetFrameRate = 60;
        [SerializeField, Range(0, 2)] private int mediumVSyncCount = 0;
        [SerializeField, Range(0.5f, 1f)] private float mediumRenderScale = 0.9f;

        [Header("High")]
        [SerializeField, Min(15)] private int highTargetFrameRate = 60;
        [SerializeField, Range(0, 2)] private int highVSyncCount = 0;
        [SerializeField, Range(0.5f, 1f)] private float highRenderScale = 1f;

        [Header("General")]
        [SerializeField] private bool keepScreenAwakeDuringGameplay = true;
        [SerializeField] private bool unloadUnusedAssetsOnLowMemory = true;
        [SerializeField, Min(0f)] private float lowMemoryCleanupCooldown = 15f;

        public MobilePerformanceTier ActiveTier => activeTier;
        public bool KeepScreenAwakeDuringGameplay =>
            keepScreenAwakeDuringGameplay;
        public bool UnloadUnusedAssetsOnLowMemory =>
            unloadUnusedAssetsOnLowMemory;
        public float LowMemoryCleanupCooldown =>
            lowMemoryCleanupCooldown;

        public int TargetFrameRate => activeTier switch
        {
            MobilePerformanceTier.Low => lowTargetFrameRate,
            MobilePerformanceTier.High => highTargetFrameRate,
            _ => mediumTargetFrameRate
        };

        public int VSyncCount => activeTier switch
        {
            MobilePerformanceTier.Low => lowVSyncCount,
            MobilePerformanceTier.High => highVSyncCount,
            _ => mediumVSyncCount
        };

        public float RenderScale => activeTier switch
        {
            MobilePerformanceTier.Low => lowRenderScale,
            MobilePerformanceTier.High => highRenderScale,
            _ => mediumRenderScale
        };

        public void SetActiveTier(MobilePerformanceTier tier)
        {
            activeTier = tier;
        }
    }
}
