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
    public static class Increment009Setup
    {
        private const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string LevelSelectionScenePath = "Assets/_Project/Scenes/LevelSelection.unity";
        private const string DifficultySelectionScenePath = "Assets/_Project/Scenes/DifficultySelection.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";

        private static readonly Color Background = new(0.12f, 0.11f, 0.09f, 1f);
        private static readonly Color Panel = new(0.20f, 0.18f, 0.14f, 0.97f);
        private static readonly Color Wood = new(0.36f, 0.20f, 0.10f, 1f);
        private static readonly Color Accent = new(1f, 0.69f, 0.03f, 1f);
        private static readonly Color Green = new(0.06f, 0.42f, 0.11f, 1f);
        private static readonly Color Red = new(0.62f, 0.10f, 0.08f, 1f);
        private static readonly Color TextPrimary = new(1f, 0.97f, 0.88f, 1f);
        private static readonly Color TextMuted = new(0.72f, 0.68f, 0.60f, 1f);
        public static void Run()
        {
            if (!File.Exists(BootstrapScenePath) || !File.Exists(LevelSelectionScenePath))
            {
                EditorUtility.DisplayDialog("Increment 009", "Run the previous increments first.", "OK");
                return;
            }

            CreateDifficultySelectionScene();
            CreateGameplayScene();
            ConfigureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ultimate Puzzles Dinos] Increment 009 generated successfully.");
            EditorUtility.DisplayDialog("Ultimate Puzzles Dinos", "Increment 009 completed. Difficulty selection is ready.", "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            ValidateScene<DifficultySelectionScreen>(DifficultySelectionScenePath, problems);
            ValidateScene<GameplayScreen>(GameplayScenePath, problems);

            string[] enabledScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            string[] expected = { BootstrapScenePath, MainMenuScenePath, LevelSelectionScenePath, DifficultySelectionScenePath, GameplayScenePath };
            foreach (string path in expected)
            {
                if (!enabledScenes.Contains(path)) problems.Add($"Scene is not enabled in Build Settings: {path}");
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 009 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 009 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 009 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }

        private static void CreateDifficultySelectionScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();
            Stretch(CreateImage("Background", safeArea, Background).rectTransform);

            Button backButton = CreateButton("BackButton", safeArea, "<  NIVELES", Wood);
            AnchorTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(38f, -34f), new Vector2(270f, 82f));

            Text title = CreateText("Title", safeArea, "SELECCION DE DIFICULTAD", 46, Accent, FontStyle.Bold);
            AnchorTopCenter(title.rectTransform, new Vector2(0f, -32f), new Vector2(850f, 70f));

            Text modeLabel = CreateText("ModeLabel", safeArea, "PUZZLE", 31, TextPrimary, FontStyle.Bold);
            AnchorTopCenter(modeLabel.rectTransform, new Vector2(0f, -105f), new Vector2(560f, 50f));

            Text levelLabel = CreateText("LevelLabel", safeArea, "NIVEL 01", 27, TextMuted, FontStyle.Bold);
            AnchorTopCenter(levelLabel.rectTransform, new Vector2(0f, -150f), new Vector2(420f, 45f));

            RectTransform cardsRoot = CreateRect("DifficultyCards", safeArea);
            cardsRoot.anchorMin = new Vector2(0.5f, 0.5f);
            cardsRoot.anchorMax = new Vector2(0.5f, 0.5f);
            cardsRoot.pivot = new Vector2(0.5f, 0.5f);
            cardsRoot.anchoredPosition = new Vector2(0f, -80f);
            cardsRoot.sizeDelta = new Vector2(1540f, 610f);
            HorizontalLayoutGroup layout = cardsRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 28f;
            layout.padding = new RectOffset(20, 20, 20, 20);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            string[] names = { "FACIL", "NORMAL", "DIFICIL", "EXPERTO" };
            string[] grids = { "3 x 3\n9 PIEZAS", "4 x 4\n16 PIEZAS", "6 x 6\n36 PIEZAS", "8 x 8\n64 PIEZAS" };
            Color[] colors = { Green, Wood, Red, Panel };
            List<Button> buttons = new();
            List<Text> labels = new();
            List<Text> gridLabels = new();

            for (int index = 0; index < names.Length; index++)
            {
                RectTransform card = CreateRect($"Difficulty_{index}", cardsRoot);
                card.sizeDelta = new Vector2(350f, 520f);
                Image cardImage = card.gameObject.AddComponent<Image>();
                cardImage.color = colors[index];
                Button button = card.gameObject.AddComponent<Button>();
                button.targetGraphic = cardImage;

                Text label = CreateText("DifficultyLabel", card, names[index], 36, TextPrimary, FontStyle.Bold);
                label.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                label.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                label.rectTransform.pivot = new Vector2(0.5f, 1f);
                label.rectTransform.anchoredPosition = new Vector2(0f, -58f);
                label.rectTransform.sizeDelta = new Vector2(300f, 70f);

                Text grid = CreateText("GridLabel", card, grids[index], 31, TextPrimary, FontStyle.Bold);
                grid.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                grid.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                grid.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                grid.rectTransform.anchoredPosition = new Vector2(0f, -5f);
                grid.rectTransform.sizeDelta = new Vector2(290f, 150f);

                Text hint = CreateText("TapHint", card, "TOCA PARA JUGAR", 21, TextMuted, FontStyle.Bold);
                hint.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                hint.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                hint.rectTransform.pivot = new Vector2(0.5f, 0f);
                hint.rectTransform.anchoredPosition = new Vector2(0f, 42f);
                hint.rectTransform.sizeDelta = new Vector2(290f, 45f);

                buttons.Add(button);
                labels.Add(label);
                gridLabels.Add(grid);
            }

            DifficultySelectionScreen controller = cardsRoot.gameObject.AddComponent<DifficultySelectionScreen>();
            SerializedObject serialized = new(controller);
            serialized.FindProperty("backButton").objectReferenceValue = backButton;
            serialized.FindProperty("modeLabel").objectReferenceValue = modeLabel;
            serialized.FindProperty("levelLabel").objectReferenceValue = levelLabel;
            SetObjectList(serialized.FindProperty("difficultyButtons"), buttons.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("difficultyLabels"), labels.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("gridLabels"), gridLabels.Cast<Object>().ToList());
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, DifficultySelectionScenePath);
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();
            Stretch(CreateImage("Background", safeArea, Background).rectTransform);

            Button backButton = CreateButton("BackButton", safeArea, "<  DIFICULTAD", Wood);
            AnchorTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(38f, -34f), new Vector2(320f, 82f));
            Button restartButton = CreateButton("RestartButton", safeArea, "REINICIAR", Panel);
            AnchorTopRight(restartButton.GetComponent<RectTransform>(), new Vector2(-38f, -34f), new Vector2(230f, 82f));

            Text modeLabel = CreateText("ModeLabel", safeArea, "PUZZLE", 35, Accent, FontStyle.Bold);
            AnchorTopCenter(modeLabel.rectTransform, new Vector2(0f, -24f), new Vector2(600f, 48f));
            Text levelLabel = CreateText("LevelLabel", safeArea, "NIVEL 01", 25, TextPrimary, FontStyle.Bold);
            AnchorTopCenter(levelLabel.rectTransform, new Vector2(0f, -67f), new Vector2(420f, 40f));
            Text difficultyLabel = CreateText("DifficultyLabel", safeArea, "FACIL", 23, Accent, FontStyle.Bold);
            AnchorTopCenter(difficultyLabel.rectTransform, new Vector2(-125f, -104f), new Vector2(240f, 38f));
            Text gridLabel = CreateText("GridLabel", safeArea, "3 x 3  ·  9 PIEZAS", 23, TextMuted, FontStyle.Bold);
            AnchorTopCenter(gridLabel.rectTransform, new Vector2(160f, -104f), new Vector2(340f, 38f));

            RectTransform frame = CreateRect("PuzzleFrame", safeArea);
            frame.anchorMin = new Vector2(0.5f, 0.5f);
            frame.anchorMax = new Vector2(0.5f, 0.5f);
            frame.pivot = new Vector2(0.5f, 0.5f);
            frame.anchoredPosition = new Vector2(0f, -70f);
            frame.sizeDelta = new Vector2(930f, 760f);
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
            serialized.FindProperty("difficultyLabel").objectReferenceValue = difficultyLabel;
            serialized.FindProperty("gridLabel").objectReferenceValue = gridLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
        }

        private static void ConfigureBuildSettings()
        {
            string[] paths = { BootstrapScenePath, MainMenuScenePath, LevelSelectionScenePath, DifficultySelectionScenePath, GameplayScenePath };
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
            bool hasInputModule = scene.GetRootGameObjects().Any(root => root.GetComponentInChildren<InputSystemUIInputModule>(true) != null);
            bool hasLegacyInput = scene.GetRootGameObjects().Any(root => root.GetComponentInChildren<StandaloneInputModule>(true) != null);
            EditorSceneManager.CloseScene(scene, true);
            if (!found) problems.Add($"{typeof(T).Name} is missing from {Path.GetFileName(path)}.");
            if (!hasInputModule) problems.Add($"InputSystemUIInputModule is missing from {Path.GetFileName(path)}.");
            if (hasLegacyInput) problems.Add($"StandaloneInputModule is still present in {Path.GetFileName(path)}.");
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

        private static void AnchorTopCenter(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
#endif
