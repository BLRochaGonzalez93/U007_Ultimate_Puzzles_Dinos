using System;
using System.Collections;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Monetization
{
    public sealed class FullscreenAdDisplayGuard : MonoBehaviour
    {
        private const int RequiredStableFrames = 4;
        private const float TimeoutSeconds = 2.5f;
        private const float SafeAreaTolerance = 1.5f;

        private static FullscreenAdDisplayGuard instance;

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            if (instance != null)
            {
                return;
            }

            GameObject root =
                new GameObject("[FullscreenAdDisplayGuard]");

            instance =
                root.AddComponent<FullscreenAdDisplayGuard>();

            DontDestroyOnLoad(root);
        }

        public static void WaitUntilReady(
            Action<bool> completed)
        {
            if (instance == null)
            {
                Create();
            }

            instance.StartCoroutine(
                instance.WaitForStableLandscape(completed));
        }

        private IEnumerator WaitForStableLandscape(
            Action<bool> completed)
        {
            float startedAt = Time.realtimeSinceStartup;
            int stableFrames = 0;

            ScreenState previous = Capture();

            while (Time.realtimeSinceStartup - startedAt <
                   TimeoutSeconds)
            {
                yield return null;

                ScreenState current = Capture();

                if (IsValidLandscape(current) &&
                    IsStable(previous, current))
                {
                    stableFrames++;

                    if (stableFrames >= RequiredStableFrames)
                    {
                        LogState(
                            "[Ads] Fullscreen display state ready",
                            current);

                        completed?.Invoke(true);
                        yield break;
                    }
                }
                else
                {
                    stableFrames = 0;
                }

                previous = current;
            }

            ScreenState timeoutState = Capture();

            LogState(
                "[Ads] Fullscreen ad blocked: screen did not " +
                "reach a stable landscape state",
                timeoutState);

            completed?.Invoke(false);
        }

        private static bool IsValidLandscape(
            ScreenState state)
        {
            if (state.Width <= 0 ||
                state.Height <= 0 ||
                state.Width <= state.Height)
            {
                return false;
            }

            if (state.SafeArea.width <= 0f ||
                state.SafeArea.height <= 0f)
            {
                return false;
            }

            if (state.SafeArea.xMin < -SafeAreaTolerance ||
                state.SafeArea.yMin < -SafeAreaTolerance ||
                state.SafeArea.xMax >
                    state.Width + SafeAreaTolerance ||
                state.SafeArea.yMax >
                    state.Height + SafeAreaTolerance)
            {
                return false;
            }

            return true;
        }

        private static bool IsStable(
            ScreenState previous,
            ScreenState current)
        {
            return
                previous.Width == current.Width &&
                previous.Height == current.Height &&
                previous.Orientation == current.Orientation &&
                Approximately(
                    previous.SafeArea,
                    current.SafeArea);
        }

        private static bool Approximately(
            Rect a,
            Rect b)
        {
            return
                Mathf.Abs(a.x - b.x) < SafeAreaTolerance &&
                Mathf.Abs(a.y - b.y) < SafeAreaTolerance &&
                Mathf.Abs(a.width - b.width) <
                    SafeAreaTolerance &&
                Mathf.Abs(a.height - b.height) <
                    SafeAreaTolerance;
        }

        private static ScreenState Capture()
        {
            return new ScreenState(
                Screen.width,
                Screen.height,
                Screen.safeArea,
                Screen.orientation);
        }

        private static void LogState(
            string prefix,
            ScreenState state)
        {
            Debug.Log(
                $"{prefix}. " +
                $"Resolution={state.Width}x{state.Height}, " +
                $"Orientation={state.Orientation}, " +
                $"SafeArea={state.SafeArea}.");
        }

        private readonly struct ScreenState
        {
            public ScreenState(
                int width,
                int height,
                Rect safeArea,
                ScreenOrientation orientation)
            {
                Width = width;
                Height = height;
                SafeArea = safeArea;
                Orientation = orientation;
            }

            public int Width { get; }
            public int Height { get; }
            public Rect SafeArea { get; }
            public ScreenOrientation Orientation { get; }
        }
    }
}
