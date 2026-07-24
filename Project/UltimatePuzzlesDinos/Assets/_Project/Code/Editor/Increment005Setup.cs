#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.UI;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment005Setup
    {
        private const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string LevelSelectionScenePath = "Assets/_Project/Scenes/LevelSelection.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";

        private static readonly Color Background = new(0.12f, 0.11f, 0.09f, 1f);
        private static readonly Color Panel = new(0.20f, 0.18f, 0.14f, 0.97f);
        private static readonly Color Wood = new(0.36f, 0.20f, 0.10f, 1f);
        private static readonly Color Accent = new(1f, 0.69f, 0.03f, 1f);
        private static readonly Color Green = new(0.06f, 0.42f, 0.11f, 1f);
        private static readonly Color Locked = new(0.20f, 0.20f, 0.20f, 1f);
        private static readonly Color TextPrimary = new(1f, 0.97f, 0.88f, 1f);
        private static readonly Color TextMuted = new(0.72f, 0.68f, 0.60f, 1f);
        public static void Run()
        {
            if (!File.Exists(BootstrapScenePath) || !File.Exists(MainMenuScenePath))
            {
                EditorUtility.DisplayDialog("Increment 005", "Run the previous increments first.", "OK");
                return;
            }

            CreateLevelSelectionScene();
            CreateGameplayScene();
            ConfigureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ultimate Puzzles Dinos] Increment 005 generated successfully.");
            EditorUtility.DisplayDialog("Ultimate Puzzles Dinos", "Increment 005 completed. Level selection is ready.", "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            ValidateScene<LevelSelectionScreen>(LevelSelectionScenePath, problems);
            ValidateScene<GameplayScreen>(GameplayScenePath, problems);

            string[] enabledScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            if (!enabledScenes.Contains(LevelSelectionScenePath))
            {
                problems.Add("LevelSelection is not enabled in Build Settings.");
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 005 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 005 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 005 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }

        private static void CreateLevelSelectionScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();
            Stretch(CreateImage("Background", safeArea, Background).rectTransform);

            Button backButton = CreateButton("BackButton", safeArea, "<  MODOS", Wood);
            AnchorTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(38f, -34f), new Vector2(250f, 82f));

            Text title = CreateText("Title", safeArea, "SELECCION DE NIVEL", 46, Accent, FontStyle.Bold);
            title.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            title.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            title.rectTransform.pivot = new Vector2(0.5f, 1f);
            title.rectTransform.anchoredPosition = new Vector2(0f, -32f);
            title.rectTransform.sizeDelta = new Vector2(760f, 70f);

            Text modeLabel = CreateText("ModeLabel", safeArea, "PUZZLE", 34, TextPrimary, FontStyle.Bold);
            modeLabel.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.anchoredPosition = new Vector2(0f, -105f);
            modeLabel.rectTransform.sizeDelta = new Vector2(620f, 60f);

            RectTransform scrollRoot = CreateRect("LevelScroll", safeArea);
            scrollRoot.anchorMin = new Vector2(0.5f, 0.5f);
            scrollRoot.anchorMax = new Vector2(0.5f, 0.5f);
            scrollRoot.pivot = new Vector2(0.5f, 0.5f);
            scrollRoot.anchoredPosition = new Vector2(0f, -55f);
            scrollRoot.sizeDelta = new Vector2(1540f, 700f);
            Image scrollBackground = scrollRoot.gameObject.AddComponent<Image>();
            scrollBackground.color = Panel;
            ScrollRect scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 45f;

            RectTransform viewport = CreateRect("Viewport", scrollRoot);
            Stretch(viewport, 24f);
            Image viewportImage = viewport.gameObject.AddComponent<Image>();
            viewportImage.color = Color.white;
            viewportImage.raycastTarget = true;
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;

            RectTransform content = CreateRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 0.5f);
            content.anchorMax = new Vector2(0f, 0.5f);
            content.pivot = new Vector2(0f, 0.5f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(12 * 275f + 11 * 28f + 60f, 610f);
            HorizontalLayoutGroup layout = content.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(30, 30, 25, 25);
            layout.spacing = 28f;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            scrollRect.viewport = viewport;
            scrollRect.content = content;

            List<Button> levelButtons = new();
            List<Text> levelLabels = new();
            List<GameObject> locks = new();

            for (int index = 0; index < 12; index++)
            {
                bool unlocked = index < 3;
                RectTransform card = CreateRect($"LevelCard_{index + 1:00}", content);
                card.sizeDelta = new Vector2(275f, 560f);
                Image cardImage = card.gameObject.AddComponent<Image>();
                cardImage.color = unlocked ? Wood : Locked;
                Button button = card.gameObject.AddComponent<Button>();
                button.targetGraphic = cardImage;

                RectTransform preview = CreateRect("Preview", card);
                preview.anchorMin = new Vector2(0.5f, 1f);
                preview.anchorMax = new Vector2(0.5f, 1f);
                preview.pivot = new Vector2(0.5f, 1f);
                preview.anchoredPosition = new Vector2(0f, -24f);
                preview.sizeDelta = new Vector2(225f, 390f);
                Image previewImage = preview.gameObject.AddComponent<Image>();
                previewImage.color = unlocked ? new Color(0.16f, 0.31f, 0.20f, 1f) : new Color(0.11f, 0.11f, 0.11f, 1f);

                Text previewText = CreateText("PreviewPlaceholder", preview, unlocked ? $"DINOSAURIO\n{index + 1:00}" : "?", 32, unlocked ? TextMuted : TextPrimary, FontStyle.Bold);
                Stretch(previewText.rectTransform, 18f);

                Text label = CreateText("LevelLabel", card, $"NIVEL {index + 1:00}", 30, TextPrimary, FontStyle.Bold);
                label.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                label.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                label.rectTransform.pivot = new Vector2(0.5f, 0f);
                label.rectTransform.anchoredPosition = new Vector2(0f, 28f);
                label.rectTransform.sizeDelta = new Vector2(230f, 76f);

                Text lockText = CreateText("LockIndicator", card, "BLOQUEADO", 25, Accent, FontStyle.Bold);
                lockText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                lockText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                lockText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                lockText.rectTransform.anchoredPosition = new Vector2(0f, -10f);
                lockText.rectTransform.sizeDelta = new Vector2(230f, 70f);
                lockText.gameObject.SetActive(!unlocked);

                levelButtons.Add(button);
                levelLabels.Add(label);
                locks.Add(lockText.gameObject);
            }

            LevelSelectionScreen controller = scrollRoot.gameObject.AddComponent<LevelSelectionScreen>();
            SerializedObject serialized = new(controller);
            serialized.FindProperty("backButton").objectReferenceValue = backButton;
            serialized.FindProperty("modeLabel").objectReferenceValue = modeLabel;
            SetObjectList(serialized.FindProperty("levelButtons"), levelButtons.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("levelLabels"), levelLabels.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("lockIndicators"), locks.Cast<Object>().ToList());
            serialized.ApplyModifiedPropertiesWithoutUndo();

            Text hint = CreateText("Hint", safeArea, "DESLIZA HORIZONTALMENTE PARA VER MAS NIVELES", 24, TextMuted, FontStyle.Normal);
            hint.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            hint.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            hint.rectTransform.pivot = new Vector2(0.5f, 0f);
            hint.rectTransform.anchoredPosition = new Vector2(0f, 28f);
            hint.rectTransform.sizeDelta = new Vector2(900f, 48f);

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, LevelSelectionScenePath);
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();
            Stretch(CreateImage("Background", safeArea, Background).rectTransform);

            Button backButton = CreateButton("BackButton", safeArea, "<  NIVELES", Wood);
            AnchorTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(38f, -34f), new Vector2(280f, 82f));
            Button restartButton = CreateButton("RestartButton", safeArea, "REINICIAR", Panel);
            AnchorTopRight(restartButton.GetComponent<RectTransform>(), new Vector2(-38f, -34f), new Vector2(230f, 82f));

            Text modeLabel = CreateText("ModeLabel", safeArea, "PUZZLE", 38, Accent, FontStyle.Bold);
            modeLabel.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.anchoredPosition = new Vector2(0f, -30f);
            modeLabel.rectTransform.sizeDelta = new Vector2(620f, 55f);

            Text levelLabel = CreateText("LevelLabel", safeArea, "NIVEL 01", 28, TextPrimary, FontStyle.Bold);
            levelLabel.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            levelLabel.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            levelLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
            levelLabel.rectTransform.anchoredPosition = new Vector2(0f, -82f);
            levelLabel.rectTransform.sizeDelta = new Vector2(420f, 50f);

            RectTransform frame = CreateRect("PuzzleFrame", safeArea);
            frame.anchorMin = new Vector2(0.5f, 0.5f);
            frame.anchorMax = new Vector2(0.5f, 0.5f);
            frame.pivot = new Vector2(0.5f, 0.5f);
            frame.anchoredPosition = new Vector2(0f, -38f);
            frame.sizeDelta = new Vector2(930f, 790f);
            frame.gameObject.AddComponent<Image>().color = Wood;

            RectTransform board = CreateRect("PuzzleBoardPlaceholder", frame);
            Stretch(board, 42f);
            board.gameObject.AddComponent<Image>().color = new Color(0.08f, 0.14f, 0.11f, 1f);
            Text placeholder = CreateText("Placeholder", board, "IMAGEN DEL NIVEL\n\nEl tablero jugable se incorporara\nen los siguientes incrementos.", 34, TextMuted, FontStyle.Bold);
            Stretch(placeholder.rectTransform, 70f);

            GameplayScreen controller = frame.gameObject.AddComponent<GameplayScreen>();
            SerializedObject serialized = new(controller);
            serialized.FindProperty("backButton").objectReferenceValue = backButton;
            serialized.FindProperty("restartButton").objectReferenceValue = restartButton;
            serialized.FindProperty("modeLabel").objectReferenceValue = modeLabel;
            serialized.FindProperty("levelLabel").objectReferenceValue = levelLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
        }

        private static void ConfigureBuildSettings()
        {
            string[] paths = { BootstrapScenePath, MainMenuScenePath, LevelSelectionScenePath, GameplayScenePath };
            EditorBuildSettings.scenes = paths.Where(File.Exists).Select(path => new EditorBuildSettingsScene(path, true)).ToArray();
        }

        private static void ValidateScene<T>(string path, List<string> problems) where T : Component
        {
            if (!File.Exists(path))
            {
                problems.Add($"Missing scene: {path}");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            bool found = scene.GetRootGameObjects().Any(root => root.GetComponentInChildren<T>(true) != null);
            EditorSceneManager.CloseScene(scene, true);
            if (!found) problems.Add($"{typeof(T).Name} is missing from {Path.GetFileName(path)}.");
        }

        private static void SetObjectList(SerializedProperty property, List<Object> objects)
        {
            property.arraySize = objects.Count;
            for (int i = 0; i < objects.Count; i++) property.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Background;
            camera.orthographic = true;
        }

        private static RectTransform CreateCanvasAndSafeArea()
        {
            GameObject canvasObject = new("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
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
            GameObject eventSystemObject = new("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            InputSystemUIActionsUtility.Configure(eventSystemObject.GetComponent<InputSystemUIInputModule>());
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
            text.color = color;
            text.fontStyle = style;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Color color)
        {
            Image image = CreateImage(name, parent, color);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            Text text = CreateText("Label", image.transform, label, 28, TextPrimary, FontStyle.Bold);
            Stretch(text.rectTransform, 12f);
            return button;
        }

        private static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(margin, margin);
            rect.offsetMax = new Vector2(-margin, -margin);
        }

        private static void AnchorTopLeft(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void AnchorTopRight(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
#endif
