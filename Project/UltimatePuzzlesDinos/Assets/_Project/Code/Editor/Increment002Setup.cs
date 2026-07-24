#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.UI;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment002Setup
    {
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";

        private static readonly Color Background = new(0.027f, 0.035f, 0.031f, 1f);
        private static readonly Color Panel = new(0.055f, 0.071f, 0.063f, 0.96f);
        private static readonly Color Primary = new(0.20f, 0.78f, 0.34f, 1f);
        private static readonly Color Secondary = new(0.15f, 0.20f, 0.17f, 1f);
        private static readonly Color TextPrimary = new(0.94f, 0.97f, 0.95f, 1f);
        private static readonly Color TextSecondary = new(0.68f, 0.74f, 0.70f, 1f);
        public static void Run()
        {
            if (!System.IO.File.Exists(MainMenuScenePath) || !System.IO.File.Exists(GameplayScenePath))
            {
                EditorUtility.DisplayDialog(
                    "Increment 002",
                    "Run Increment 001 first. MainMenu.unity or Gameplay.unity is missing.",
                    "OK");
                return;
            }

            CreateMainMenuScene();
            CreateGameplayScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Ultimate Puzzles Dinos] Increment 002 generated successfully.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 002 completed. Main menu and gameplay navigation are ready.",
                "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            ValidateSceneComponent<MainMenuScreen>(MainMenuScenePath, problems);
            ValidateSceneComponent<GameplayScreen>(GameplayScenePath, problems);

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 002 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 002 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 002 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }

        private static void CreateMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();

            Image background = CreateImage("Background", safeArea, Background);
            Stretch(background.rectTransform);

            RectTransform content = CreateRect("MainMenuContent", safeArea);
            content.anchorMin = new Vector2(0.5f, 0.5f);
            content.anchorMax = new Vector2(0.5f, 0.5f);
            content.pivot = new Vector2(0.5f, 0.5f);
            content.sizeDelta = new Vector2(720f, 720f);

            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(72, 72, 64, 64);
            layout.spacing = 24f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Image panel = content.gameObject.AddComponent<Image>();
            panel.color = Panel;

            Text title = CreateText("Title", content, "ULTIMATE PUZZLES DINOS", 54, TextPrimary, FontStyle.Bold);
            SetPreferredHeight(title.gameObject, 110f);

            Text subtitle = CreateText("Subtitle", content, "Puzzle adventure · UI prototype", 28, TextSecondary, FontStyle.Normal);
            SetPreferredHeight(subtitle.gameObject, 64f);

            Button playButton = CreateButton("PlayButton", content, "JUGAR", Primary, TextPrimary);
            SetPreferredHeight(playButton.gameObject, 104f);

            Button settingsButton = CreateButton("SettingsButton", content, "AJUSTES (PRÓXIMAMENTE)", Secondary, TextSecondary);
            settingsButton.interactable = false;
            SetPreferredHeight(settingsButton.gameObject, 88f);

            Button quitButton = CreateButton("QuitButton", content, "SALIR", Secondary, TextPrimary);
            SetPreferredHeight(quitButton.gameObject, 88f);

            MainMenuScreen screen = content.gameObject.AddComponent<MainMenuScreen>();
            SerializedObject serialized = new(screen);
            serialized.FindProperty("playButton").objectReferenceValue = playButton;
            serialized.FindProperty("quitButton").objectReferenceValue = quitButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();

            Image background = CreateImage("Background", safeArea, Background);
            Stretch(background.rectTransform);

            RectTransform header = CreateRect("Header", safeArea);
            header.anchorMin = new Vector2(0f, 1f);
            header.anchorMax = new Vector2(1f, 1f);
            header.pivot = new Vector2(0.5f, 1f);
            header.sizeDelta = new Vector2(0f, 132f);
            header.anchoredPosition = Vector2.zero;
            Image headerImage = header.gameObject.AddComponent<Image>();
            headerImage.color = Panel;

            HorizontalLayoutGroup headerLayout = header.gameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.padding = new RectOffset(32, 32, 22, 22);
            headerLayout.spacing = 24f;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childControlWidth = false;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = true;

            Button backButton = CreateButton("BackButton", header, "VOLVER", Secondary, TextPrimary);
            SetPreferredSize(backButton.gameObject, 230f, 82f);

            Text title = CreateText("Title", header, "GAMEPLAY · PROTOTIPO", 36, TextPrimary, FontStyle.Bold);
            LayoutElement titleLayout = title.gameObject.AddComponent<LayoutElement>();
            titleLayout.flexibleWidth = 1f;
            title.alignment = TextAnchor.MiddleCenter;

            Button restartButton = CreateButton("RestartButton", header, "REINICIAR", Secondary, TextPrimary);
            SetPreferredSize(restartButton.gameObject, 230f, 82f);

            RectTransform boardArea = CreateRect("BoardArea", safeArea);
            boardArea.anchorMin = new Vector2(0.5f, 0.5f);
            boardArea.anchorMax = new Vector2(0.5f, 0.5f);
            boardArea.pivot = new Vector2(0.5f, 0.5f);
            boardArea.sizeDelta = new Vector2(820f, 650f);
            boardArea.anchoredPosition = new Vector2(0f, -42f);
            Image boardPanel = boardArea.gameObject.AddComponent<Image>();
            boardPanel.color = Panel;

            Text placeholder = CreateText("Placeholder", boardArea,
                "ÁREA DEL PUZZLE\n\nEl tablero y las piezas se añadirán en los próximos incrementos.",
                34, TextSecondary, FontStyle.Normal);
            Stretch(placeholder.rectTransform, 60f);
            placeholder.alignment = TextAnchor.MiddleCenter;

            GameplayScreen screen = boardArea.gameObject.AddComponent<GameplayScreen>();
            SerializedObject serialized = new(screen);
            serialized.FindProperty("backButton").objectReferenceValue = backButton;
            serialized.FindProperty("restartButton").objectReferenceValue = restartButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
        }

        private static void ValidateSceneComponent<T>(string scenePath, List<string> problems) where T : Component
        {
            if (!System.IO.File.Exists(scenePath))
            {
                problems.Add($"Missing scene: {scenePath}");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            if (Object.FindFirstObjectByType<T>() == null)
            {
                problems.Add($"{scene.name} does not contain {typeof(T).Name}.");
            }
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new("Main Camera", typeof(Camera));
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Background;
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";
        }

        private static RectTransform CreateCanvasAndSafeArea()
        {
            GameObject canvasObject = new("AppCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform safeArea = CreateRect("SafeArea", canvasObject.transform);
            Stretch(safeArea);
            safeArea.gameObject.AddComponent<SafeAreaFitter>();
            return safeArea;
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystem = new("EventSystem", typeof(EventSystem));
            System.Type inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
            {
                eventSystem.AddComponent(inputModuleType);
            }
            else
            {
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject gameObject = new(name, typeof(RectTransform));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            RectTransform rect = CreateRect(name, parent);
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text CreateText(string name, Transform parent, string value, int size, Color color, FontStyle style)
        {
            RectTransform rect = CreateRect(name, parent);
            Text text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Color background, Color foreground)
        {
            Image image = CreateImage(name, parent, background);
            Button button = image.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.90f, 0.96f, 0.92f, 1f);
            colors.pressedColor = new Color(0.78f, 0.86f, 0.80f, 1f);
            colors.disabledColor = new Color(0.50f, 0.54f, 0.51f, 0.60f);
            button.colors = colors;

            Text text = CreateText("Label", image.transform, label, 30, foreground, FontStyle.Bold);
            Stretch(text.rectTransform, 18f);
            return button;
        }

        private static void SetPreferredHeight(GameObject target, float height)
        {
            LayoutElement element = target.GetComponent<LayoutElement>() ?? target.AddComponent<LayoutElement>();
            element.preferredHeight = height;
        }

        private static void SetPreferredSize(GameObject target, float width, float height)
        {
            LayoutElement element = target.GetComponent<LayoutElement>() ?? target.AddComponent<LayoutElement>();
            element.preferredWidth = width;
            element.preferredHeight = height;
        }

        private static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(margin, margin);
            rect.offsetMax = new Vector2(-margin, -margin);
        }
    }
}
#endif
