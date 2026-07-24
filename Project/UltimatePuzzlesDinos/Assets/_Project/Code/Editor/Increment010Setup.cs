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
using VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle;
using VRMGames.UltimatePuzzlesDinos.UI;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment010Setup
    {
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";
        private static readonly Color Background = new(0.12f, 0.11f, 0.09f, 1f);
        private static readonly Color Panel = new(0.20f, 0.18f, 0.14f, 0.97f);
        private static readonly Color Wood = new(0.36f, 0.20f, 0.10f, 1f);
        private static readonly Color Accent = new(1f, 0.69f, 0.03f, 1f);
        private static readonly Color TextPrimary = new(1f, 0.97f, 0.88f, 1f);
        private static readonly Color TextMuted = new(0.72f, 0.68f, 0.60f, 1f);
        public static void Run()
        {
            if (!File.Exists(GameplayScenePath))
            {
                EditorUtility.DisplayDialog("Increment 010", "Run Increment 009 first.", "OK");
                return;
            }

            CreateGameplayScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ultimate Puzzles Dinos] Increment 010 generated successfully.");
            EditorUtility.DisplayDialog("Ultimate Puzzles Dinos", "Increment 010 completed. The first playable puzzle board is ready.", "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            if (!File.Exists(GameplayScenePath))
            {
                problems.Add($"Missing scene: {GameplayScenePath}");
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Additive);
                GameObject[] roots = scene.GetRootGameObjects();
                if (!roots.Any(root => root.GetComponentInChildren<GameplayScreen>(true) != null)) problems.Add("GameplayScreen is missing.");
                if (!roots.Any(root => root.GetComponentInChildren<PuzzleBoardController>(true) != null)) problems.Add("PuzzleBoardController is missing.");
                if (!roots.Any(root => root.GetComponentInChildren<InputSystemUIInputModule>(true) != null)) problems.Add("InputSystemUIInputModule is missing.");
                if (roots.Any(root => root.GetComponentInChildren<StandaloneInputModule>(true) != null)) problems.Add("StandaloneInputModule is still present.");
                EditorSceneManager.CloseScene(scene, true);
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 010 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 010 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 010 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }

        private static void CreateGameplayScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateCamera();
            RectTransform safeArea = CreateCanvasAndSafeArea(out Canvas canvas);
            Stretch(CreateImage("Background", safeArea, Background).rectTransform);

            Button backButton = CreateButton("BackButton", safeArea, "<  DIFICULTAD", Wood);
            AnchorTopLeft(backButton.GetComponent<RectTransform>(), new Vector2(32f, -28f), new Vector2(300f, 76f));
            Button restartButton = CreateButton("RestartButton", safeArea, "REINICIAR", Panel);
            AnchorTopRight(restartButton.GetComponent<RectTransform>(), new Vector2(-32f, -28f), new Vector2(220f, 76f));

            Text modeLabel = CreateText("ModeLabel", safeArea, "PUZZLE", 33, Accent, FontStyle.Bold);
            AnchorTopCenter(modeLabel.rectTransform, new Vector2(0f, -18f), new Vector2(560f, 44f));
            Text levelLabel = CreateText("LevelLabel", safeArea, "NIVEL 01", 23, TextPrimary, FontStyle.Bold);
            AnchorTopCenter(levelLabel.rectTransform, new Vector2(0f, -57f), new Vector2(380f, 36f));
            Text difficultyLabel = CreateText("DifficultyLabel", safeArea, "FACIL", 21, Accent, FontStyle.Bold);
            AnchorTopCenter(difficultyLabel.rectTransform, new Vector2(-125f, -92f), new Vector2(220f, 34f));
            Text gridLabel = CreateText("GridLabel", safeArea, "3 x 3 · 9 PIEZAS", 21, TextMuted, FontStyle.Bold);
            AnchorTopCenter(gridLabel.rectTransform, new Vector2(150f, -92f), new Vector2(330f, 34f));

            RectTransform content = CreateRect("GameplayContent", safeArea);
            content.anchorMin = new Vector2(0f, 0f);
            content.anchorMax = new Vector2(1f, 1f);
            content.offsetMin = new Vector2(36f, 34f);
            content.offsetMax = new Vector2(-36f, -145f);

            RectTransform boardFrame = CreateRect("BoardFrame", content);
            boardFrame.anchorMin = new Vector2(0f, 0.5f);
            boardFrame.anchorMax = new Vector2(0f, 0.5f);
            boardFrame.pivot = new Vector2(0f, 0.5f);
            boardFrame.anchoredPosition = new Vector2(30f, 0f);
            boardFrame.sizeDelta = new Vector2(760f, 760f);
            boardFrame.gameObject.AddComponent<Image>().color = Wood;

            RectTransform boardRoot = CreateRect("BoardRoot", boardFrame);
            Stretch(boardRoot, 28f);
            boardRoot.gameObject.AddComponent<Image>().color = new Color(0.06f, 0.08f, 0.06f, 1f);

            RectTransform trayFrame = CreateRect("TrayFrame", content);
            trayFrame.anchorMin = new Vector2(1f, 0.5f);
            trayFrame.anchorMax = new Vector2(1f, 0.5f);
            trayFrame.pivot = new Vector2(1f, 0.5f);
            trayFrame.anchoredPosition = new Vector2(-30f, 0f);
            trayFrame.sizeDelta = new Vector2(760f, 760f);
            trayFrame.gameObject.AddComponent<Image>().color = Panel;

            Text trayTitle = CreateText("TrayTitle", trayFrame, "PIEZAS", 28, Accent, FontStyle.Bold);
            trayTitle.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            trayTitle.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            trayTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
            trayTitle.rectTransform.anchoredPosition = new Vector2(0f, -18f);
            trayTitle.rectTransform.sizeDelta = new Vector2(300f, 44f);

            Text statusLabel = CreateText("StatusLabel", trayFrame, "COLOCADAS: 0 / 9", 22, TextMuted, FontStyle.Bold);
            statusLabel.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            statusLabel.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            statusLabel.rectTransform.pivot = new Vector2(0.5f, 1f);
            statusLabel.rectTransform.anchoredPosition = new Vector2(0f, -60f);
            statusLabel.rectTransform.sizeDelta = new Vector2(420f, 38f);

            RectTransform trayRoot = CreateRect("TrayRoot", trayFrame);
            trayRoot.anchorMin = Vector2.zero;
            trayRoot.anchorMax = Vector2.one;
            trayRoot.offsetMin = new Vector2(24f, 24f);
            trayRoot.offsetMax = new Vector2(-24f, -110f);

            RectTransform boardControllerRoot = CreateRect("StandardPuzzleController", content);
            Stretch(boardControllerRoot);
            PuzzleBoardController boardController = boardControllerRoot.gameObject.AddComponent<PuzzleBoardController>();

            GameObject completionPanel = CreateImage("CompletionPanel", safeArea, new Color(0.03f, 0.04f, 0.03f, 0.94f)).gameObject;
            RectTransform completionRect = completionPanel.GetComponent<RectTransform>();
            completionRect.anchorMin = new Vector2(0.5f, 0.5f);
            completionRect.anchorMax = new Vector2(0.5f, 0.5f);
            completionRect.pivot = new Vector2(0.5f, 0.5f);
            completionRect.sizeDelta = new Vector2(760f, 430f);
            Text completionTitle = CreateText("Title", completionRect, "PUZZLE COMPLETADO", 48, Accent, FontStyle.Bold);
            completionTitle.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            completionTitle.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            completionTitle.rectTransform.pivot = new Vector2(0.5f, 1f);
            completionTitle.rectTransform.anchoredPosition = new Vector2(0f, -70f);
            completionTitle.rectTransform.sizeDelta = new Vector2(650f, 80f);
            Text completionText = CreateText("Message", completionRect, "Has colocado todas las piezas.", 28, TextPrimary, FontStyle.Normal);
            completionText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            completionText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            completionText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            completionText.rectTransform.anchoredPosition = new Vector2(0f, 15f);
            completionText.rectTransform.sizeDelta = new Vector2(620f, 70f);
            Button completionRestart = CreateButton("RestartButton", completionRect, "JUGAR DE NUEVO", Wood);
            completionRestart.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
            completionRestart.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0f);
            completionRestart.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
            completionRestart.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 55f);
            completionRestart.GetComponent<RectTransform>().sizeDelta = new Vector2(360f, 78f);
            completionPanel.SetActive(false);

            RectTransform unsupportedPanel = CreateRect("UnsupportedModePanel", content);
            Stretch(unsupportedPanel);
            unsupportedPanel.gameObject.AddComponent<Image>().color = Panel;
            Text unsupportedLabel = CreateText("UnsupportedModeLabel", unsupportedPanel, "MECANICA EN PREPARACION", 42, Accent, FontStyle.Bold);
            Stretch(unsupportedLabel.rectTransform, 80f);
            unsupportedPanel.gameObject.SetActive(false);

            SerializedObject boardSerialized = new(boardController);
            boardSerialized.FindProperty("rootCanvas").objectReferenceValue = canvas;
            boardSerialized.FindProperty("boardRoot").objectReferenceValue = boardRoot;
            boardSerialized.FindProperty("trayRoot").objectReferenceValue = trayRoot;
            boardSerialized.FindProperty("statusLabel").objectReferenceValue = statusLabel;
            boardSerialized.FindProperty("completionPanel").objectReferenceValue = completionPanel;
            boardSerialized.FindProperty("completionRestartButton").objectReferenceValue = completionRestart;
            boardSerialized.ApplyModifiedPropertiesWithoutUndo();

            GameplayScreen screen = boardControllerRoot.gameObject.AddComponent<GameplayScreen>();
            SerializedObject screenSerialized = new(screen);
            screenSerialized.FindProperty("backButton").objectReferenceValue = backButton;
            screenSerialized.FindProperty("restartButton").objectReferenceValue = restartButton;
            screenSerialized.FindProperty("modeLabel").objectReferenceValue = modeLabel;
            screenSerialized.FindProperty("levelLabel").objectReferenceValue = levelLabel;
            screenSerialized.FindProperty("difficultyLabel").objectReferenceValue = difficultyLabel;
            screenSerialized.FindProperty("gridLabel").objectReferenceValue = gridLabel;
            screenSerialized.FindProperty("boardController").objectReferenceValue = boardController;
            screenSerialized.FindProperty("unsupportedModePanel").objectReferenceValue = unsupportedPanel.gameObject;
            screenSerialized.FindProperty("unsupportedModeLabel").objectReferenceValue = unsupportedLabel;
            screenSerialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
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

        private static RectTransform CreateCanvasAndSafeArea(out Canvas canvas)
        {
            GameObject canvasObject = new("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
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
            Text text = CreateText("Label", image.transform, label, 26, TextPrimary, FontStyle.Bold);
            Stretch(text.rectTransform, 10f);
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
