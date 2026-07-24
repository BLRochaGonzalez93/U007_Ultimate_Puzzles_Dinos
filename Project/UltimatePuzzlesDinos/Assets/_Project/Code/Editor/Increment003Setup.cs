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
    public static class Increment003Setup
    {
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";

        private static readonly Color Background = new(0.027f, 0.035f, 0.031f, 1f);
        private static readonly Color Panel = new(0.055f, 0.071f, 0.063f, 0.98f);
        private static readonly Color Overlay = new(0f, 0f, 0f, 0.72f);
        private static readonly Color Primary = new(0.20f, 0.78f, 0.34f, 1f);
        private static readonly Color Secondary = new(0.15f, 0.20f, 0.17f, 1f);
        private static readonly Color TextPrimary = new(0.94f, 0.97f, 0.95f, 1f);
        private static readonly Color TextSecondary = new(0.68f, 0.74f, 0.70f, 1f);
        public static void Run()
        {
            if (!System.IO.File.Exists(MainMenuScenePath))
            {
                EditorUtility.DisplayDialog("Increment 003", "Run Increment 001 and 002 first. MainMenu.unity is missing.", "OK");
                return;
            }

            CreateMainMenuScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Ultimate Puzzles Dinos] Increment 003 generated successfully.");
            EditorUtility.DisplayDialog("Ultimate Puzzles Dinos", "Increment 003 completed. Settings and persistence are ready.", "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();

            if (!System.IO.File.Exists(MainMenuScenePath))
            {
                problems.Add($"Missing scene: {MainMenuScenePath}");
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
                if (Object.FindFirstObjectByType<MainMenuScreen>() == null) problems.Add($"{scene.name} does not contain MainMenuScreen.");
                if (Object.FindFirstObjectByType<SettingsPanel>(FindObjectsInactive.Include) == null) problems.Add($"{scene.name} does not contain SettingsPanel.");
                bool hasSettingsOverlay = false;
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                    {
                        if (child.name == "SettingsOverlay")
                        {
                            hasSettingsOverlay = true;
                            break;
                        }
                    }

                    if (hasSettingsOverlay) break;
                }

                if (!hasSettingsOverlay) problems.Add($"{scene.name} does not contain SettingsOverlay.");
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 003 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 003 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 003 validation failed:\n- " + report);
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

            Button settingsButton = CreateButton("SettingsButton", content, "AJUSTES", Secondary, TextPrimary);
            SetPreferredHeight(settingsButton.gameObject, 88f);

            Button quitButton = CreateButton("QuitButton", content, "SALIR", Secondary, TextPrimary);
            SetPreferredHeight(quitButton.gameObject, 88f);

            SettingsPanel settingsPanel = CreateSettingsOverlay(safeArea);

            MainMenuScreen screen = content.gameObject.AddComponent<MainMenuScreen>();
            SerializedObject serialized = new(screen);
            serialized.FindProperty("playButton").objectReferenceValue = playButton;
            serialized.FindProperty("settingsButton").objectReferenceValue = settingsButton;
            serialized.FindProperty("quitButton").objectReferenceValue = quitButton;
            serialized.FindProperty("settingsPanel").objectReferenceValue = settingsPanel;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            CreateEventSystem();
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
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
            panelImage.color = Panel;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(64, 64, 48, 48);
            layout.spacing = 22f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Text title = CreateText("Title", panel, "AJUSTES", 48, TextPrimary, FontStyle.Bold);
            SetPreferredHeight(title.gameObject, 86f);

            Slider musicSlider = CreateSliderRow(panel, "MÚSICA", out Text musicValueLabel);
            Slider sfxSlider = CreateSliderRow(panel, "EFECTOS", out Text sfxValueLabel);
            Toggle vibrationToggle = CreateToggleRow(panel, "VIBRACIÓN");

            Button resetButton = CreateButton("ResetButton", panel, "RESTABLECER", Secondary, TextPrimary);
            SetPreferredHeight(resetButton.gameObject, 78f);

            Button closeButton = CreateButton("CloseButton", panel, "CERRAR", Primary, TextPrimary);
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
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            Text nameLabel = CreateText("Label", row, label, 28, TextPrimary, FontStyle.Bold);
            SetPreferredSize(nameLabel.gameObject, 190f, 82f);

            Slider slider = CreateSlider("Slider", row);
            SetPreferredSize(slider.gameObject, 390f, 56f);

            valueLabel = CreateText("Value", row, "0%", 28, TextSecondary, FontStyle.Bold);
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
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

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

            Image background = CreateImage("Background", root, Secondary);
            Stretch(background.rectTransform, 10f);

            RectTransform fillArea = CreateRect("Fill Area", root);
            Stretch(fillArea, 10f);
            Image fill = CreateImage("Fill", fillArea, Primary);
            Stretch(fill.rectTransform);

            RectTransform handleArea = CreateRect("Handle Slide Area", root);
            Stretch(handleArea, 10f);
            Image handle = CreateImage("Handle", handleArea, TextPrimary);
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
            Image background = CreateImage(name, parent, Secondary);
            Toggle toggle = background.gameObject.AddComponent<Toggle>();
            Image checkmark = CreateImage("Checkmark", background.transform, Primary);
            Stretch(checkmark.rectTransform, 12f);
            toggle.targetGraphic = background;
            toggle.graphic = checkmark;
            return toggle;
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
