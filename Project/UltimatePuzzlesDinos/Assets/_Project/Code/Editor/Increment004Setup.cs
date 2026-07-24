#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
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
    public static class Increment004Setup
    {
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";

        private static readonly Color CaveBackground = new(0.12f, 0.11f, 0.09f, 1f);
        private static readonly Color CavePanel = new(0.20f, 0.18f, 0.14f, 0.96f);
        private static readonly Color Wood = new(0.36f, 0.20f, 0.10f, 1f);
        private static readonly Color PuzzleRed = new(0.55f, 0.08f, 0.08f, 1f);
        private static readonly Color LogicGold = new(0.52f, 0.39f, 0.03f, 1f);
        private static readonly Color MosaicGreen = new(0.06f, 0.42f, 0.11f, 1f);
        private static readonly Color Accent = new(1f, 0.69f, 0.03f, 1f);
        private static readonly Color TextPrimary = new(1f, 0.97f, 0.88f, 1f);
        private static readonly Color TextMuted = new(0.77f, 0.72f, 0.62f, 1f);
        private static readonly Color Overlay = new(0f, 0f, 0f, 0.72f);
        public static void Run()
        {
            if (!File.Exists(MainMenuScenePath) || !File.Exists(GameplayScenePath))
            {
                EditorUtility.DisplayDialog(
                    "Increment 004",
                    "Run the previous increments first. MainMenu.unity or Gameplay.unity is missing.",
                    "OK");
                return;
            }

            CreateMainMenuScene();
            CreateGameplayScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Ultimate Puzzles Dinos] Increment 004 generated successfully.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 004 completed. Game mode selection and themed layout are ready.",
                "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            ValidateScene<MainMenuScreen>(MainMenuScenePath, problems);
            ValidateScene<GameplayScreen>(GameplayScenePath, problems);

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 004 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 004 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 004 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }

        private static void CreateMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();

            Image background = CreateImage("CaveBackground", safeArea, CaveBackground);
            Stretch(background.rectTransform);

            CreateDecorativePuzzlePieces(safeArea);

            RectTransform logoArea = CreateRect("LogoArea", safeArea);
            logoArea.anchorMin = new Vector2(0.5f, 1f);
            logoArea.anchorMax = new Vector2(0.5f, 1f);
            logoArea.pivot = new Vector2(0.5f, 1f);
            logoArea.anchoredPosition = new Vector2(0f, -65f);
            logoArea.sizeDelta = new Vector2(960f, 280f);

            Text logo = CreateText("LogoPlaceholder", logoArea,
                "ULTIMATE  DINOSAURS\nPUZZLE\nADVENTURE",
                58, Accent, FontStyle.Bold);
            Stretch(logo.rectTransform);
            logo.lineSpacing = 0.8f;

            RectTransform modes = CreateRect("ModeSelection", safeArea);
            modes.anchorMin = new Vector2(0.5f, 0.5f);
            modes.anchorMax = new Vector2(0.5f, 0.5f);
            modes.pivot = new Vector2(0.5f, 0.5f);
            modes.anchoredPosition = new Vector2(0f, -110f);
            modes.sizeDelta = new Vector2(1370f, 390f);

            HorizontalLayoutGroup modeLayout = modes.gameObject.AddComponent<HorizontalLayoutGroup>();
            modeLayout.padding = new RectOffset(35, 35, 20, 20);
            modeLayout.spacing = 90f;
            modeLayout.childAlignment = TextAnchor.MiddleCenter;
            modeLayout.childControlWidth = false;
            modeLayout.childControlHeight = false;
            modeLayout.childForceExpandWidth = false;
            modeLayout.childForceExpandHeight = false;

            Button puzzleButton = CreateModeButton("PuzzleButton", modes, "PUZZLE", PuzzleRed);
            Button logicButton = CreateModeButton("PuzzleLogicButton", modes, "PUZZLE\nLOGIC", LogicGold);
            Button mosaicButton = CreateModeButton("MosaicButton", modes, "MOSAIC", MosaicGreen);

            Button settingsButton = CreateButton("SettingsButton", safeArea, "AJUSTES", new Color(0.25f, 0.19f, 0.38f, 1f), TextPrimary);
            AnchorBottomLeft(settingsButton.GetComponent<RectTransform>(), new Vector2(48f, 44f), new Vector2(260f, 86f));

            Button quitButton = CreateButton("QuitButton", safeArea, "SALIR", CavePanel, TextPrimary);
            AnchorBottomRight(quitButton.GetComponent<RectTransform>(), new Vector2(-48f, 44f), new Vector2(220f, 86f));

            SettingsPanel settingsPanel = CreateSettingsOverlay(safeArea);

            MainMenuScreen controller = modes.gameObject.AddComponent<MainMenuScreen>();
            SerializedObject serialized = new(controller);
            serialized.FindProperty("puzzleButton").objectReferenceValue = puzzleButton;
            serialized.FindProperty("puzzleLogicButton").objectReferenceValue = logicButton;
            serialized.FindProperty("mosaicButton").objectReferenceValue = mosaicButton;
            serialized.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            serialized.FindProperty("quitButton").objectReferenceValue = quitButton;
            serialized.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea();

            Image background = CreateImage("CaveBackground", safeArea, CaveBackground);
            Stretch(background.rectTransform);
            CreateDecorativePuzzlePieces(safeArea);

            Button backButton = CreateButton("BackButton", safeArea, "◀  LEVEL SELECTION", Wood, TextPrimary);
            AnchorTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(38f, -34f), new Vector2(350f, 88f));

            Button restartButton = CreateButton("RestartButton", safeArea, "REINICIAR", CavePanel, TextPrimary);
            AnchorTopRight(restartButton.GetComponent<RectTransform>(), new Vector2(-38f, -34f), new Vector2(230f, 88f));

            Text modeLabel = CreateText("ModeLabel", safeArea, "PUZZLE", 42, Accent, FontStyle.Bold);
            modeLabel.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
            modeLabel.rectTransform.anchoredPosition = new Vector2(0f, -45f);
            modeLabel.rectTransform.sizeDelta = new Vector2(620f, 80f);

            RectTransform frame = CreateRect("PuzzleFrame", safeArea);
            frame.anchorMin = new Vector2(0.5f, 0.5f);
            frame.anchorMax = new Vector2(0.5f, 0.5f);
            frame.pivot = new Vector2(0.5f, 0.5f);
            frame.anchoredPosition = new Vector2(0f, -32f);
            frame.sizeDelta = new Vector2(930f, 790f);
            Image frameImage = frame.gameObject.AddComponent<Image>();
            frameImage.color = Wood;

            RectTransform board = CreateRect("PuzzleBoardPlaceholder", frame);
            Stretch(board, 42f);
            Image boardImage = board.gameObject.AddComponent<Image>();
            boardImage.color = new Color(0.08f, 0.14f, 0.11f, 1f);

            Text placeholder = CreateText("Placeholder", board,
                "IMAGEN DEL PUZZLE\n\nEl catálogo, selección de nivel y tablero jugable\nse incorporarán en los próximos incrementos.",
                34, TextMuted, FontStyle.Bold);
            Stretch(placeholder.rectTransform, 70f);

            GameplayScreen controller = frame.gameObject.AddComponent<GameplayScreen>();
            SerializedObject serialized = new(controller);
            serialized.FindProperty("backButton").objectReferenceValue = backButton;
            serialized.FindProperty("restartButton").objectReferenceValue = restartButton;
            serialized.FindProperty("modeLabel").objectReferenceValue = modeLabel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
        }

        private static Button CreateModeButton(string name, Transform parent, string label, Color color)
        {
            Image image = CreateImage(name, parent, color);
            RectTransform rect = image.rectTransform;
            rect.sizeDelta = new Vector2(350f, 300f);

            Button button = image.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.08f, 1.08f, 1.08f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            button.colors = colors;

            Text text = CreateText("Label", image.transform, label, 40, TextPrimary, FontStyle.Bold);
            Stretch(text.rectTransform, 24f);
            return button;
        }

        private static SettingsPanel CreateSettingsOverlay(RectTransform safeArea)
        {
            Image overlay = CreateImage("SettingsOverlay", safeArea, Overlay);
            Stretch(overlay.rectTransform);

            RectTransform panel = CreateRect("SettingsPanel", overlay.transform);
            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(820f, 700f);
            Image panelImage = panel.gameObject.AddComponent<Image>();
            panelImage.color = CavePanel;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(64, 64, 48, 48);
            layout.spacing = 22f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Text title = CreateText("Title", panel, "AJUSTES", 48, Accent, FontStyle.Bold);
            SetPreferredHeight(title.gameObject, 86f);

            Slider musicSlider = CreateSliderRow(panel, "MÚSICA", out Text musicValueLabel);
            Slider sfxSlider = CreateSliderRow(panel, "EFECTOS", out Text sfxValueLabel);
            Toggle vibrationToggle = CreateToggleRow(panel, "VIBRACIÓN");

            Button resetButton = CreateButton("ResetButton", panel, "RESTABLECER", CavePanel, TextPrimary);
            SetPreferredHeight(resetButton.gameObject, 78f);

            Button closeButton = CreateButton("CloseButton", panel, "CERRAR", MosaicGreen, TextPrimary);
            SetPreferredHeight(closeButton.gameObject, 88f);

            SettingsPanel controller = panel.gameObject.AddComponent<SettingsPanel>();
            SerializedObject serialized = new(controller);
            serialized.FindProperty("panelRoot").objectReferenceValue = overlay.gameObject;
            serialized.FindProperty("musicSlider").objectReferenceValue = musicSlider;
            serialized.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
            serialized.FindProperty("vibrationToggle").objectReferenceValue = vibrationToggle;
            serialized.FindProperty("musicValueLabel").objectReferenceValue = musicValueLabel;
            serialized.FindProperty("sfxValueLabel").objectReferenceValue = sfxValueLabel;
            serialized.FindProperty("closeButton").objectReferenceValue = closeButton;
            serialized.FindProperty("resetButton").objectReferenceValue = resetButton;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            overlay.gameObject.SetActive(false);
            return controller;
        }

        private static Slider CreateSliderRow(Transform parent, string label, out Text valueLabel)
        {
            RectTransform row = CreateRect(label + "Row", parent);
            SetPreferredHeight(row.gameObject, 100f);
            HorizontalLayoutGroup layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = true;

            Text nameLabel = CreateText("Label", row, label, 28, TextPrimary, FontStyle.Bold);
            SetPreferredSize(nameLabel.gameObject, 190f, 82f);

            Slider slider = CreateSlider("Slider", row);
            SetPreferredSize(slider.gameObject, 390f, 56f);

            valueLabel = CreateText("Value", row, "0%", 28, TextMuted, FontStyle.Bold);
            SetPreferredSize(valueLabel.gameObject, 100f, 82f);
            return slider;
        }

        private static Toggle CreateToggleRow(Transform parent, string label)
        {
            RectTransform row = CreateRect(label + "Row", parent);
            SetPreferredHeight(row.gameObject, 90f);
            HorizontalLayoutGroup layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 24f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = true;

            Text nameLabel = CreateText("Label", row, label, 28, TextPrimary, FontStyle.Bold);
            SetPreferredSize(nameLabel.gameObject, 580f, 74f);

            Toggle toggle = CreateToggle("Toggle", row);
            SetPreferredSize(toggle.gameObject, 74f, 74f);
            return toggle;
        }

        private static Slider CreateSlider(string name, Transform parent)
        {
            RectTransform root = CreateRect(name, parent);
            Slider slider = root.gameObject.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;

            Image background = CreateImage("Background", root, CavePanel);
            Stretch(background.rectTransform, 10f);

            RectTransform fillArea = CreateRect("Fill Area", root);
            Stretch(fillArea, 10f);
            Image fill = CreateImage("Fill", fillArea, MosaicGreen);
            Stretch(fill.rectTransform);

            RectTransform handleArea = CreateRect("Handle Slide Area", root);
            Stretch(handleArea, 10f);
            Image handle = CreateImage("Handle", handleArea, Accent);
            handle.rectTransform.sizeDelta = new Vector2(34f, 34f);

            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            return slider;
        }

        private static Toggle CreateToggle(string name, Transform parent)
        {
            Image background = CreateImage(name, parent, CavePanel);
            Toggle toggle = background.gameObject.AddComponent<Toggle>();
            Image checkmark = CreateImage("Checkmark", background.transform, MosaicGreen);
            Stretch(checkmark.rectTransform, 12f);
            toggle.targetGraphic = background;
            toggle.graphic = checkmark;
            return toggle;
        }

        private static void CreateDecorativePuzzlePieces(Transform parent)
        {
            Color[] colors = { PuzzleRed, LogicGold, MosaicGreen, new Color(0.35f, 0.12f, 0.50f, 0.8f) };
            Vector2[] anchors =
            {
                new(0.08f, 0.78f), new(0.20f, 0.25f), new(0.36f, 0.82f), new(0.49f, 0.18f),
                new(0.63f, 0.78f), new(0.77f, 0.25f), new(0.90f, 0.68f), new(0.88f, 0.12f)
            };

            for (int i = 0; i < anchors.Length; i++)
            {
                Image piece = CreateImage($"DecorativePiece_{i + 1:00}", parent, colors[i % colors.Length]);
                RectTransform rect = piece.rectTransform;
                rect.anchorMin = anchors[i];
                rect.anchorMax = anchors[i];
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(42f + (i % 3) * 10f, 42f + (i % 3) * 10f);
                rect.localRotation = Quaternion.Euler(0f, 0f, i * 19f);
                piece.raycastTarget = false;
            }
        }

        private static void CreateCamera()
        {
            GameObject cameraObject = new("Main Camera", typeof(Camera));
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = CaveBackground;
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
            if (inputModuleType != null) eventSystem.AddComponent(inputModuleType);
            else eventSystem.AddComponent<StandaloneInputModule>();
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
            Text text = CreateText("Label", image.transform, label, 28, foreground, FontStyle.Bold);
            Stretch(text.rectTransform, 14f);
            return button;
        }

        private static void AnchorBottomLeft(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void AnchorBottomRight(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
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
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(margin, margin);
            rect.offsetMax = new Vector2(-margin, -margin);
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

        private static void ValidateScene<T>(string path, List<string> problems) where T : Object
        {
            if (!File.Exists(path))
            {
                problems.Add($"Missing scene: {path}");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            if (Object.FindFirstObjectByType<T>(FindObjectsInactive.Include) == null)
            {
                problems.Add($"{scene.name} does not contain {typeof(T).Name}.");
            }
        }
    }
}
#endif
