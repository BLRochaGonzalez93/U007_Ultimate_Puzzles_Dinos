#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Application;
using VRMGames.UltimatePuzzlesDinos.Configuration;
using VRMGames.UltimatePuzzlesDinos.UI;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment001Setup
    {
        private const string RootFolder = "Assets/_Project";
        private const string ConfigFolder = RootFolder + "/Config/Editions";
        private const string ScenesFolder = RootFolder + "/Scenes";
        private const string FreeConfigPath = ConfigFolder + "/Edition_Free.asset";
        private const string PremiumConfigPath = ConfigFolder + "/Edition_Premium.asset";
        private const string BootstrapScenePath = ScenesFolder + "/Bootstrap.unity";
        private const string MainMenuScenePath = ScenesFolder + "/MainMenu.unity";
        private const string GameplayScenePath = ScenesFolder + "/Gameplay.unity";
        public static void Run()
        {
            EnsureFolders();

            EditionConfig freeConfig = CreateOrUpdateEditionConfig(
                FreeConfigPath,
                GameEdition.Free,
                enableAds: true,
                enableRewardedUnlocks: true,
                unlockAllContent: false,
                unlockedPuzzleCount: 3);

            CreateOrUpdateEditionConfig(
                PremiumConfigPath,
                GameEdition.Premium,
                enableAds: false,
                enableRewardedUnlocks: false,
                unlockAllContent: true,
                unlockedPuzzleCount: 0);

            CreateBootstrapScene(freeConfig);
            CreatePlaceholderScene(MainMenuScenePath, "Main Menu", new Color(0.035f, 0.045f, 0.04f, 1f));
            CreatePlaceholderScene(GameplayScenePath, "Gameplay", new Color(0.025f, 0.035f, 0.03f, 1f));
            ConfigureBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Ultimate Puzzles Dinos] Increment 001 generated successfully.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 001 completed. Bootstrap, MainMenu, Gameplay and both edition configurations were generated.",
                "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();

            ValidateAsset<EditionConfig>(FreeConfigPath, problems);
            ValidateAsset<EditionConfig>(PremiumConfigPath, problems);
            ValidateScene(BootstrapScenePath, problems);
            ValidateScene(MainMenuScenePath, problems);
            ValidateScene(GameplayScenePath, problems);

            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length < 3)
            {
                problems.Add("Build Settings does not contain the three required scenes.");
            }
            else
            {
                ValidateBuildScene(buildScenes, 0, BootstrapScenePath, problems);
                ValidateBuildScene(buildScenes, 1, MainMenuScenePath, problems);
                ValidateBuildScene(buildScenes, 2, GameplayScenePath, problems);
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 001 validation passed.");
                EditorUtility.DisplayDialog(
                    "Validation passed",
                    "Increment 001 is correctly installed.",
                    "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 001 validation failed:\n- " + report);
            EditorUtility.DisplayDialog(
                "Validation failed",
                "The following problems were found:\n\n- " + report,
                "OK");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder(RootFolder, "Art");
            EnsureFolder(RootFolder + "/Art", "Audio");
            EnsureFolder(RootFolder + "/Art/Audio", "Music");
            EnsureFolder(RootFolder + "/Art/Audio", "SFX");
            EnsureFolder(RootFolder + "/Art", "Sprites");
            EnsureFolder(RootFolder + "/Art/Sprites", "Puzzles");
            EnsureFolder(RootFolder + "/Art/Sprites", "UI");
            EnsureFolder(RootFolder, "Config");
            EnsureFolder(RootFolder + "/Config", "Editions");
            EnsureFolder(RootFolder, "Prefabs");
            EnsureFolder(RootFolder, "Scenes");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string fullPath = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static EditionConfig CreateOrUpdateEditionConfig(
            string assetPath,
            GameEdition edition,
            bool enableAds,
            bool enableRewardedUnlocks,
            bool unlockAllContent,
            int unlockedPuzzleCount)
        {
            EditionConfig config = AssetDatabase.LoadAssetAtPath<EditionConfig>(assetPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EditionConfig>();
                AssetDatabase.CreateAsset(config, assetPath);
            }

            config.Configure(
                edition,
                enableAds,
                enableRewardedUnlocks,
                unlockAllContent,
                unlockedPuzzleCount);

            EditorUtility.SetDirty(config);
            return config;
        }

        private static void CreateBootstrapScene(EditionConfig freeConfig)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Bootstrap";

            GameObject bootstrapObject = new("AppBootstrap");
            AppBootstrap bootstrap = bootstrapObject.AddComponent<AppBootstrap>();

            SerializedObject serializedBootstrap = new(bootstrap);
            serializedBootstrap.FindProperty("editionConfig").objectReferenceValue = freeConfig;
            serializedBootstrap.FindProperty("loadMainMenuOnStart").boolValue = true;
            serializedBootstrap.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, BootstrapScenePath);
        }

        private static void CreatePlaceholderScene(string path, string title, Color backgroundColor)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = backgroundColor;
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";

            GameObject canvasObject = new("AppCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            GameObject safeAreaObject = new("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
            RectTransform safeArea = safeAreaObject.GetComponent<RectTransform>();
            safeArea.SetParent(canvasObject.transform, false);
            safeArea.anchorMin = Vector2.zero;
            safeArea.anchorMax = Vector2.one;
            safeArea.offsetMin = Vector2.zero;
            safeArea.offsetMax = Vector2.zero;

            GameObject placeholder = new("ScreenPlaceholder", typeof(RectTransform));
            RectTransform placeholderRect = placeholder.GetComponent<RectTransform>();
            placeholderRect.SetParent(safeArea, false);
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;

            GameObject marker = new(title.Replace(" ", string.Empty) + "Root");
            marker.transform.SetParent(placeholder.transform, false);

            GameObject eventSystemObject = new("EventSystem", typeof(EventSystem));
            AddCompatibleInputModule(eventSystemObject);

            EditorSceneManager.SaveScene(scene, path);
        }

        private static void AddCompatibleInputModule(GameObject eventSystemObject)
        {
            System.Type inputSystemModuleType = System.Type.GetType(
                "UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");

            if (inputSystemModuleType != null)
            {
                eventSystemObject.AddComponent(inputSystemModuleType);
                return;
            }

            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootstrapScenePath, true),
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(GameplayScenePath, true)
            };
        }

        private static void ValidateAsset<T>(string path, ICollection<string> problems)
            where T : Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
            {
                problems.Add("Missing asset: " + path);
            }
        }

        private static void ValidateScene(string path, ICollection<string> problems)
        {
            if (!File.Exists(path))
            {
                problems.Add("Missing scene: " + path);
            }
        }

        private static void ValidateBuildScene(
            IReadOnlyList<EditorBuildSettingsScene> scenes,
            int index,
            string expectedPath,
            ICollection<string> problems)
        {
            if (scenes[index].path != expectedPath || !scenes[index].enabled)
            {
                problems.Add("Incorrect Build Settings entry at index " + index + ": " + expectedPath);
            }
        }
    }
}
#endif
