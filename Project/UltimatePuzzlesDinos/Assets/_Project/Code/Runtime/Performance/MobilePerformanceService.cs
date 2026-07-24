using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VRMGames.UltimatePuzzlesDinos.Performance
{
    [DisallowMultipleComponent]
    public sealed class MobilePerformanceService : MonoBehaviour
    {
        private const string ConfigResourcePath =
            "Performance/MobilePerformanceConfig";

        private static MobilePerformanceService instance;

        private MobilePerformanceConfig config;
        private float lastLowMemoryCleanupTime = -999f;

        public static MobilePerformanceService Instance => instance;

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateService()
        {
            if (instance != null)
            {
                return;
            }

            GameObject serviceObject = new(
                "[MobilePerformanceService]");

            instance =
                serviceObject.AddComponent<MobilePerformanceService>();

            DontDestroyOnLoad(serviceObject);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            config = Resources.Load<MobilePerformanceConfig>(
                ConfigResourcePath);

            ApplyActiveConfiguration();
            UnityEngine.Application.lowMemory += HandleLowMemory;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                UnityEngine.Application.lowMemory -= HandleLowMemory;
                instance = null;
            }
        }

        public void ApplyActiveConfiguration()
        {
            if (config == null)
            {
                Debug.LogWarning(
                    "[Performance] MobilePerformanceConfig not found. " +
                    "Using Unity defaults.");

                return;
            }

            QualitySettings.vSyncCount = config.VSyncCount;
            UnityEngine.Application.targetFrameRate = config.TargetFrameRate;

            Screen.sleepTimeout =
                config.KeepScreenAwakeDuringGameplay
                    ? SleepTimeout.NeverSleep
                    : SleepTimeout.SystemSetting;

            UniversalRenderPipelineAsset urpAsset =
                QualitySettings.renderPipeline
                    as UniversalRenderPipelineAsset;

            if (urpAsset != null)
            {
                urpAsset.renderScale =
                    Mathf.Clamp(config.RenderScale, 0.5f, 1f);
            }

            Debug.Log(
                $"[Performance] Tier={config.ActiveTier}, " +
                $"FPS={config.TargetFrameRate}, " +
                $"RenderScale={config.RenderScale:0.00}, " +
                $"VSync={config.VSyncCount}.");
        }

        private void HandleLowMemory()
        {
            if (config == null ||
                !config.UnloadUnusedAssetsOnLowMemory)
            {
                return;
            }

            if (Time.unscaledTime - lastLowMemoryCleanupTime <
                config.LowMemoryCleanupCooldown)
            {
                return;
            }

            lastLowMemoryCleanupTime = Time.unscaledTime;
            StartCoroutine(CleanupUnusedAssets());
        }

        private static IEnumerator CleanupUnusedAssets()
        {
            Debug.LogWarning(
                "[Performance] Low-memory signal received. " +
                "Unloading unused assets.");

            AsyncOperation operation =
                Resources.UnloadUnusedAssets();

            while (!operation.isDone)
            {
                yield return null;
            }

            System.GC.Collect();
        }
    }
}
