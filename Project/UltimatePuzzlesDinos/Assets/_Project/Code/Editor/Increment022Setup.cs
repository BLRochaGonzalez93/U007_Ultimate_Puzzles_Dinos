#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.UI.Effects;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment022Setup
    {
        private const string UiRoot = "Assets/_Project/Art/Sprites/UI/";
        private const string MainMenuPath = "Assets/_Project/Scenes/MainMenu.unity";
        private const string LevelSelectionPath = "Assets/_Project/Scenes/LevelSelection.unity";
        private const string DifficultySelectionPath = "Assets/_Project/Scenes/DifficultySelection.unity";
        private const string GameplayPath = "Assets/_Project/Scenes/Gameplay.unity";

        public static void Run()
        {
            List<string> problems = ValidateAssets();
            if (problems.Count > 0)
            {
                EditorUtility.DisplayDialog(
                    "Increment 022",
                    "Faltan recursos requeridos:\n\n" + string.Join("\n", problems),
                    "OK");
                return;
            }

            PatchMainMenu();
            PatchLevelSelection();
            PatchDifficultySelection();
            PatchGameplay();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Increment 022] Instalación completada.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 022 completado.\n\nSe han aplicado las mejoras visuales y de UX solicitadas.",
                "OK");
        }

        public static void Validate()
        {
            List<string> problems = ValidateAssets();

            string[] requiredScripts =
            {
                "Assets/_Project/Code/Runtime/UI/Effects/FloatingImageAnimator.cs",
                "Assets/_Project/Code/Runtime/UI/Effects/ContinuousRotateUI.cs",
                "Assets/_Project/Code/Runtime/UI/Effects/PulseScaleUI.cs",
                "Assets/_Project/Code/Runtime/UI/Screens/LevelSelectionVisualOverride.cs",
                "Assets/_Project/Code/Runtime/UI/Screens/SettingsPanelVisualLayout.cs",
                "Assets/_Project/Code/Runtime/Gameplay/Puzzle/PuzzleLogicBoardController.cs",
                "Assets/_Project/Code/Editor/Increment022Setup.cs"
            };

            foreach (string path in requiredScripts)
            {
                if (!File.Exists(path))
                {
                    problems.Add(path);
                }
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Increment 022] Validación correcta.");
                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 022 instalado correctamente.",
                    "OK");
                return;
            }

            Debug.LogError("[Increment 022] Validación fallida:\n- " + string.Join("\n- ", problems));
            EditorUtility.DisplayDialog(
                "Validación fallida",
                string.Join("\n", problems),
                "OK");
        }

        private static List<string> ValidateAssets()
        {
            string[] names =
            {
                "Pieza Cute.png",
                "Radial Shine.png",
                "frame_gray.png",
                "Gradient.png",
                "LockedGallery.png"
            };

            return names
                .Where(name => AssetDatabase.LoadAssetAtPath<Sprite>(UiRoot + name) == null)
                .Select(name => UiRoot + name)
                .ToList();
        }

        private static void PatchMainMenu()
        {
            Scene scene = EditorSceneManager.OpenScene(MainMenuPath, OpenSceneMode.Single);
            Transform root = GetSceneRoot(scene);

            Sprite frameGray = LoadSprite("frame_gray.png");
            ConfigureMenuModeButton(Find(root, "PuzzleButton"), frameGray, new Color(0.36f, 0.74f, 0.35f, 1f));
            ConfigureMenuModeButton(Find(root, "PuzzleLogicButton"), frameGray, new Color(0.31f, 0.58f, 0.96f, 1f));
            ConfigureMenuModeButton(Find(root, "MosaicButton"), frameGray, new Color(0.63f, 0.39f, 0.92f, 1f));

            ApplyMainMenuDecor(root);
            ApplySettingsPanelFix(root);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchLevelSelection()
        {
            Scene scene = EditorSceneManager.OpenScene(LevelSelectionPath, OpenSceneMode.Single);
            Transform root = GetSceneRoot(scene);

            EnsureComponent<LevelSelectionVisualOverride>(Find(root, "Canvas") ?? root);

            for (int i = 1; i <= 60; i++)
            {
                Transform card = Find(root, $"LevelCard_{i:00}");
                if (card == null)
                {
                    continue;
                }

                EnsureComponent<RectMask2D>(card);

                RectTransform preview = Find(card, "Preview") as RectTransform;
                if (preview != null)
                {
                    preview.anchorMin = Vector2.zero;
                    preview.anchorMax = Vector2.one;
                    preview.offsetMin = new Vector2(18f, 18f);
                    preview.offsetMax = new Vector2(-18f, -18f);
                    preview.SetSiblingIndex(0);
                    Image previewImage = preview.GetComponent<Image>();
                    if (previewImage != null)
                    {
                        previewImage.preserveAspect = true;
                    }
                }

                RectTransform levelLabel = Find(card, "LevelLabel") as RectTransform;
                if (levelLabel != null)
                {
                    levelLabel.anchorMin = new Vector2(0.5f, 1f);
                    levelLabel.anchorMax = new Vector2(0.5f, 1f);
                    levelLabel.pivot = new Vector2(0.5f, 1f);
                    levelLabel.anchoredPosition = new Vector2(0f, -6f);
                    levelLabel.sizeDelta = new Vector2(124f, 28f);
                    levelLabel.SetSiblingIndex(card.childCount - 1);
                }

                Transform placeholder = Find(card, "PreviewPlaceholder");
                if (placeholder != null)
                {
                    placeholder.gameObject.SetActive(false);
                    Text text = placeholder.GetComponent<Text>();
                    if (text != null)
                    {
                        text.text = string.Empty;
                        text.enabled = false;
                    }
                }

                Transform lockIndicator = Find(card, "LockIndicator");
                if (lockIndicator != null)
                {
                    Text lockText = lockIndicator.GetComponent<Text>();
                    if (lockText != null)
                    {
                        lockText.text = string.Empty;
                        lockText.enabled = false;
                    }

                    RectTransform lockRect = lockIndicator as RectTransform;
                    if (lockRect != null)
                    {
                        lockRect.anchorMin = Vector2.zero;
                        lockRect.anchorMax = Vector2.one;
                        lockRect.offsetMin = new Vector2(18f, 18f);
                        lockRect.offsetMax = new Vector2(-18f, -18f);
                        lockRect.SetSiblingIndex(card.childCount - 1);
                    }

                    Image cover = CreateOrUpdateChildImage(
                        lockIndicator,
                        "LockCover",
                        LoadSprite("LockedGallery.png"),
                        new Color(1f, 1f, 1f, 0.42f),
                        true);

                    if (cover != null)
                    {
                        RectTransform coverRect = cover.transform as RectTransform;
                        coverRect.anchorMin = Vector2.zero;
                        coverRect.anchorMax = Vector2.one;
                        coverRect.offsetMin = Vector2.zero;
                        coverRect.offsetMax = Vector2.zero;
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchDifficultySelection()
        {
            Scene scene = EditorSceneManager.OpenScene(DifficultySelectionPath, OpenSceneMode.Single);
            Transform root = GetSceneRoot(scene);

            SetImage(Find(root, "Background"), LoadSprite("Gradient.png"), Color.white, false);

            Sprite frameGray = LoadSprite("frame_gray.png");
            Color[] colors =
            {
                new Color(0.35f, 0.74f, 0.39f, 1f),
                new Color(0.22f, 0.67f, 0.92f, 1f),
                new Color(0.96f, 0.63f, 0.24f, 1f),
                new Color(0.71f, 0.42f, 0.96f, 1f)
            };

            for (int index = 0; index < 4; index++)
            {
                Transform button = Find(root, $"Difficulty_{index}");
                if (button == null)
                {
                    continue;
                }

                ConfigureMenuModeButton(button, frameGray, colors[Mathf.Clamp(index, 0, colors.Length - 1)]);
                PulseScaleUI pulse = EnsureComponent<PulseScaleUI>(button);
                SetPrivateField(pulse, "phaseOffset", index * 0.45f);
            }

            List<Transform> tapHints = FindAll(root, t => t.name == "TapHint");
            for (int i = 0; i < tapHints.Count; i++)
            {
                Text text = tapHints[i].GetComponent<Text>();
                if (i == 0)
                {
                    tapHints[i].gameObject.SetActive(true);
                    RectTransform rect = tapHints[i] as RectTransform;
                    if (rect != null)
                    {
                        rect.anchorMin = new Vector2(0.5f, 0f);
                        rect.anchorMax = new Vector2(0.5f, 0f);
                        rect.pivot = new Vector2(0.5f, 0f);
                        rect.anchoredPosition = new Vector2(0f, 24f);
                        rect.sizeDelta = new Vector2(560f, 42f);
                    }

                    if (text != null)
                    {
                        text.text = "TOCA UN BOTÓN PARA JUGAR";
                        text.alignment = TextAnchor.MiddleCenter;
                        text.fontStyle = FontStyle.Bold;
                    }
                }
                else
                {
                    tapHints[i].gameObject.SetActive(false);
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchGameplay()
        {
            Scene scene = EditorSceneManager.OpenScene(GameplayPath, OpenSceneMode.Single);
            Transform root = GetSceneRoot(scene);
            SetImage(Find(root, "Background"), LoadSprite("Gradient.png"), Color.white, false);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ApplyMainMenuDecor(Transform root)
        {
            Sprite cutePiece = LoadSprite("Pieza Cute.png");
            Sprite radial = LoadSprite("Radial Shine.png");
            Transform logoArea = Find(root, "LogoArea") ?? root;

            Transform radialShine = Find(logoArea, "RadialShine");
            if (radialShine == null)
            {
                GameObject go = new("RadialShine", typeof(RectTransform), typeof(Image));
                radialShine = go.transform;
                radialShine.SetParent(logoArea, false);
            }

            RectTransform radialRect = radialShine as RectTransform;
            radialRect.anchorMin = new Vector2(0.5f, 0.5f);
            radialRect.anchorMax = new Vector2(0.5f, 0.5f);
            radialRect.pivot = new Vector2(0.5f, 0.5f);
            radialRect.anchoredPosition = new Vector2(0f, -8f);
            radialRect.sizeDelta = new Vector2(760f, 760f);
            radialRect.SetSiblingIndex(0);
            Image radialImage = radialShine.GetComponent<Image>();
            radialImage.sprite = radial;
            radialImage.color = new Color(1f, 1f, 1f, 0.20f);
            radialImage.raycastTarget = false;
            EnsureComponent<ContinuousRotateUI>(radialShine);

            Vector2[] positions =
            {
                new(-420f, 240f), new(420f, 230f), new(-520f, 30f), new(520f, 20f),
                new(-410f, -210f), new(410f, -230f), new(-560f, -90f), new(560f, 110f)
            };

            for (int i = 1; i <= 8; i++)
            {
                Transform piece = Find(root, $"DecorativePiece_{i:00}");
                if (piece == null)
                {
                    GameObject go = new($"DecorativePiece_{i:00}", typeof(RectTransform), typeof(Image));
                    piece = go.transform;
                    piece.SetParent(Find(root, "SafeArea") ?? root, false);
                }

                Image image = piece.GetComponent<Image>();
                if (image == null)
                {
                    image = piece.gameObject.AddComponent<Image>();
                }

                image.sprite = cutePiece;
                image.color = Color.white;
                image.preserveAspect = true;
                image.raycastTarget = false;

                RectTransform rect = piece as RectTransform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = positions[(i - 1) % positions.Length];
                rect.sizeDelta = new Vector2(84f, 84f);

                FloatingImageAnimator animator = EnsureComponent<FloatingImageAnimator>(piece);
                SetPrivateField(animator, "phaseOffset", i * 0.7f);
                SetPrivateField(animator, "movementAmplitude", new Vector2(20f + i * 2f, 10f + (i % 3) * 3f));
                SetPrivateField(animator, "movementFrequency", new Vector2(0.35f + i * 0.03f, 0.28f + i * 0.02f));
            }
        }

        private static void ApplySettingsPanelFix(Transform root)
        {
            Transform panel = Find(root, "SettingsPanel");
            if (panel == null)
            {
                return;
            }

            RectTransform panelRect = panel as RectTransform;
            if (panelRect != null)
            {
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(760f, 620f);
                panelRect.anchoredPosition = Vector2.zero;
            }

            EnsureComponent<SettingsPanelVisualLayout>(panel);
        }

        private static void ConfigureMenuModeButton(Transform button, Sprite sprite, Color tint)
        {
            if (button == null)
            {
                return;
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Simple;
                image.color = tint;
                image.preserveAspect = false;
            }

            Text[] texts = button.GetComponentsInChildren<Text>(true);
            foreach (Text text in texts)
            {
                text.color = Color.white;
                text.fontStyle = FontStyle.Bold;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 14;
                text.resizeTextMaxSize = 34;
            }
        }

        private static Sprite LoadSprite(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(UiRoot + fileName);
        }

        private static void SetImage(Transform target, Sprite sprite, Color color, bool preserveAspect)
        {
            if (target == null)
            {
                return;
            }

            Image image = target.GetComponent<Image>();
            if (image == null)
            {
                Graphic existingGraphic = target.GetComponent<Graphic>();
                if (existingGraphic != null)
                {
                    return;
                }

                image = target.gameObject.AddComponent<Image>();
            }

            image.sprite = sprite;
            image.color = color;
            image.type = Image.Type.Simple;
            image.preserveAspect = preserveAspect;
        }

        private static Image CreateOrUpdateChildImage(Transform parent, string childName, Sprite sprite, Color color, bool preserveAspect)
        {
            Transform child = parent.Find(childName);
            if (child == null)
            {
                GameObject go = new(childName, typeof(RectTransform), typeof(Image));
                child = go.transform;
                child.SetParent(parent, false);
            }

            Image image = child.GetComponent<Image>();
            if (image == null)
            {
                image = child.gameObject.AddComponent<Image>();
            }

            image.sprite = sprite;
            image.color = color;
            image.type = Image.Type.Simple;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
            return image;
        }

        private static T EnsureComponent<T>(Transform target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (component == null)
            {
                component = target.gameObject.AddComponent<T>();
            }
            return component;
        }

        private static Transform GetSceneRoot(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            Transform canvas = roots
                .SelectMany(root => root.GetComponentsInChildren<Canvas>(true))
                .Select(item => item.transform)
                .FirstOrDefault();
            return canvas != null ? canvas : roots.FirstOrDefault()?.transform;
        }

        private static Transform Find(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root;
            }

            foreach (Transform child in root)
            {
                Transform result = Find(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static List<Transform> FindAll(Transform root, Func<Transform, bool> predicate)
        {
            List<Transform> results = new();
            Collect(root, predicate, results);
            return results;
        }

        private static void Collect(Transform root, Func<Transform, bool> predicate, List<Transform> results)
        {
            if (root == null)
            {
                return;
            }

            if (predicate(root))
            {
                results.Add(root);
            }

            foreach (Transform child in root)
            {
                Collect(child, predicate, results);
            }
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            if (target == null)
            {
                return;
            }

            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty((UnityEngine.Object)target);
            }
        }
    }
}
#endif
