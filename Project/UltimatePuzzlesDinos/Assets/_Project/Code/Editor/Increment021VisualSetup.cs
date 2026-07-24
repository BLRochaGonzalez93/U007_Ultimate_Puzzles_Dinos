#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment021VisualSetup
    {
        private const string UiRoot =
            "Assets/_Project/Art/Sprites/UI/";

        private static readonly string[] ScenePaths =
        {
            "Assets/_Project/Scenes/MainMenu.unity",
            "Assets/_Project/Scenes/LevelSelection.unity",
            "Assets/_Project/Scenes/DifficultySelection.unity",
            "Assets/_Project/Scenes/Gameplay.unity"
        };

        public static void Run()
        {
            List<string> missingAssets = ValidateRequiredAssets();

            if (missingAssets.Count > 0)
            {
                Debug.LogError(
                    "[Increment 021] Faltan recursos visuales:\n- " +
                    string.Join("\n- ", missingAssets));

                EditorUtility.DisplayDialog(
                    "Increment 021",
                    "Faltan recursos visuales requeridos.\n\n" +
                    string.Join("\n", missingAssets),
                    "OK");

                return;
            }

            PatchMainMenu();
            PatchLevelSelection();
            PatchDifficultySelection();
            PatchGameplay();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "[Ultimate Puzzles Dinos] Increment 021 installed. " +
                "Legacy-inspired visual theme applied.");

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 021 completado.\n\n" +
                "Se ha aplicado la primera versión visual definitiva.",
                "OK");
        }

        public static void Validate()
        {
            List<string> problems = ValidateRequiredAssets();

            foreach (string scenePath in ScenePaths)
            {
                if (!File.Exists(scenePath))
                {
                    problems.Add($"Falta la escena: {scenePath}");
                }
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Increment 021] Validación correcta.");

                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 021 instalado correctamente.",
                    "OK");

                return;
            }

            Debug.LogError(
                "[Increment 021] Validación fallida:\n- " +
                string.Join("\n- ", problems));

            EditorUtility.DisplayDialog(
                "Validación fallida",
                string.Join("\n", problems),
                "OK");
        }

        private static void PatchMainMenu()
        {
            const string scenePath =
                "Assets/_Project/Scenes/MainMenu.unity";

            Scene scene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Single);

            Transform root = GetSceneRoot(scene);

            SetImage(
                Find(root, "Background"),
                LoadSprite("Violet.png"),
                new Color(0.34f, 0.18f, 0.42f, 1f),
                false);

            Transform logoArea = Find(root, "LogoArea");
            if (logoArea != null)
            {
                ClearGeneratedLogo(logoArea);
                CreateLogoImage(
                    logoArea,
                    "UltimateLogo",
                    LoadSprite("Ultimate-13-6-2024.png"),
                    new Vector2(0f, 92f),
                    new Vector2(700f, 155f));

                CreateLogoImage(
                    logoArea,
                    "PuzzleLogo",
                    LoadSprite("puzzle-12-6-2024-transformed.png"),
                    new Vector2(0f, 0f),
                    new Vector2(600f, 145f));

                CreateLogoImage(
                    logoArea,
                    "DinosaursLogo",
                    LoadSprite("DINOSAURS-28-6-2024.png"),
                    new Vector2(0f, -95f),
                    new Vector2(650f, 120f));

                Transform placeholder = Find(logoArea, "LogoPlaceholder");
                if (placeholder != null)
                {
                    placeholder.gameObject.SetActive(false);
                }
            }

            StyleButton(
                Find(root, "PuzzleButton"),
                LoadSprite("Game 1.png"),
                Color.white,
                Color.white);

            StyleButton(
                Find(root, "PuzzleLogicButton"),
                LoadSprite("Game 2.png"),
                Color.white,
                Color.white);

            StyleButton(
                Find(root, "MosaicButton"),
                LoadSprite("Game 3.png"),
                Color.white,
                Color.white);

            StyleButton(
                Find(root, "SettingsButton"),
                LoadSprite("btn_back_gray.png"),
                Color.white,
                Color.white);

            StyleButton(
                Find(root, "QuitButton"),
                LoadSprite("btn_forward_gray.png"),
                Color.white,
                Color.white);

            StylePanel(
                Find(root, "SettingsPanel"),
                LoadSprite("popup_bg.png"),
                Color.white);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchLevelSelection()
        {
            const string scenePath =
                "Assets/_Project/Scenes/LevelSelection.unity";

            Scene scene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Single);

            Transform root = GetSceneRoot(scene);

            SetImage(
                Find(root, "Background"),
                LoadSprite("Gradient.png"),
                new Color(0.30f, 0.16f, 0.10f, 1f),
                false);

            StyleButton(
                Find(root, "BackButton"),
                LoadSprite("btn_back_gray.png"),
                Color.white,
                Color.white);

            for (int level = 1; level <= 60; level++)
            {
                Transform card = Find(root, $"LevelCard_{level:00}");
                if (card == null)
                {
                    continue;
                }

                SetImage(
                    card,
                    LoadSprite("MarcoDino-transformed (2).png"),
                    Color.white,
                    true);

                Image preview = Find(card, "Preview")
                    ?.GetComponent<Image>();

                if (preview != null)
                {
                    preview.preserveAspect = true;
                }

                Transform lockIndicator = Find(card, "LockIndicator");
                if (lockIndicator != null)
                {
                    CreateOrUpdateChildImage(
                        lockIndicator,
                        "LockIcon",
                        LoadSprite("LockedGallery.png"),
                        Color.white,
                        Vector2.zero,
                        new Vector2(72f, 72f));

                    Text lockText = lockIndicator.GetComponent<Text>();
                    if (lockText != null)
                    {
                        lockText.text = string.Empty;
                        lockText.raycastTarget = false;
                    }
                }

                Text levelLabel = Find(card, "LevelLabel")
                    ?.GetComponent<Text>();

                if (levelLabel != null)
                {
                    levelLabel.color =
                        new Color(1f, 0.87f, 0.36f, 1f);
                    levelLabel.fontStyle = FontStyle.Bold;
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchDifficultySelection()
        {
            const string scenePath =
                "Assets/_Project/Scenes/DifficultySelection.unity";

            Scene scene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Single);

            Transform root = GetSceneRoot(scene);

            SetImage(
                Find(root, "Background"),
                LoadSprite("Gradient.png"),
                new Color(0.25f, 0.14f, 0.08f, 1f),
                false);

            StyleButton(
                Find(root, "BackButton"),
                LoadSprite("btn_back_gray.png"),
                Color.white,
                Color.white);

            for (int index = 0; index < 4; index++)
            {
                Transform card = Find(root, $"Difficulty_{index}");
                if (card == null)
                {
                    continue;
                }

                Sprite sprite = index switch
                {
                    0 => LoadSprite("btn_item_green.png"),
                    1 => LoadSprite("btn_mission_default.png"),
                    2 => LoadSprite("Btn_Upgrade_n.png"),
                    _ => LoadSprite("btn_message_blank.png")
                };

                StyleButton(
                    card,
                    sprite,
                    Color.white,
                    Color.white);

                Text difficultyLabel = Find(card, "DifficultyLabel")
                    ?.GetComponent<Text>();

                if (difficultyLabel != null)
                {
                    difficultyLabel.fontStyle = FontStyle.Bold;
                    difficultyLabel.color = Color.white;
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchGameplay()
        {
            const string scenePath =
                "Assets/_Project/Scenes/Gameplay.unity";

            Scene scene = EditorSceneManager.OpenScene(
                scenePath,
                OpenSceneMode.Single);

            Transform root = GetSceneRoot(scene);

            SetImage(
                Find(root, "Background"),
                LoadSprite("Violet.png"),
                new Color(0.23f, 0.13f, 0.28f, 1f),
                false);

            StyleButton(
                Find(root, "BackButton"),
                LoadSprite("btn_back_gray.png"),
                Color.white,
                Color.white);

            StyleButton(
                Find(root, "RestartButton"),
                LoadSprite("btn_forward_gray.png"),
                Color.white,
                Color.white);

            SetImage(
                Find(root, "BoardFrame"),
                LoadSprite("MarcoDino-transformed (2).png"),
                Color.white,
                true);

            SetImage(
                Find(root, "TrayFrame"),
                LoadSprite("Marco.png"),
                Color.white,
                true);

            SetImage(
                Find(root, "CompletionPanel"),
                LoadSprite("popup_bg.png"),
                Color.white,
                true);

            Text modeLabel = Find(root, "ModeLabel")
                ?.GetComponent<Text>();

            if (modeLabel != null)
            {
                modeLabel.color =
                    new Color(1f, 0.73f, 0.16f, 1f);
                modeLabel.fontStyle = FontStyle.Bold;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static List<string> ValidateRequiredAssets()
        {
            string[] names =
            {
                "Ultimate-13-6-2024.png",
                "puzzle-12-6-2024-transformed.png",
                "DINOSAURS-28-6-2024.png",
                "Game 1.png",
                "Game 2.png",
                "Game 3.png",
                "MarcoDino-transformed (2).png",
                "Marco.png",
                "popup_bg.png",
                "btn_back_gray.png",
                "btn_forward_gray.png",
                "btn_item_green.png",
                "btn_mission_default.png",
                "Btn_Upgrade_n.png",
                "btn_message_blank.png",
                "LockedGallery.png",
                "Gradient.png",
                "Violet.png"
            };

            return names
                .Where(name =>
                    AssetDatabase.LoadAssetAtPath<Sprite>(
                        UiRoot + name) == null)
                .Select(name => UiRoot + name)
                .ToList();
        }

        private static Sprite LoadSprite(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(
                UiRoot + fileName);
        }

        private static Transform GetSceneRoot(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();

            Transform canvas = roots
                .SelectMany(root =>
                    root.GetComponentsInChildren<Canvas>(true))
                .Select(item => item.transform)
                .FirstOrDefault();

            return canvas != null
                ? canvas
                : roots.FirstOrDefault()?.transform;
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

        private static void StyleButton(
            Transform target,
            Sprite sprite,
            Color imageColor,
            Color textColor)
        {
            if (target == null)
            {
                return;
            }

            Image image = target.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
                image.color = imageColor;
            }

            Text[] labels =
                target.GetComponentsInChildren<Text>(true);

            foreach (Text label in labels)
            {
                label.color = textColor;
                label.fontStyle = FontStyle.Bold;
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 12;
                label.resizeTextMaxSize =
                    Mathf.Max(label.fontSize, 28);
            }
        }

        private static void StylePanel(
            Transform target,
            Sprite sprite,
            Color color)
        {
            SetImage(target, sprite, color, true);
        }

        private static void SetImage(
            Transform target,
            Sprite sprite,
            Color color,
            bool preserveAspect)
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
                    Debug.LogWarning(
                        $"[Increment 021] No se puede añadir Image a {target.name} " +
                        $"porque ya contiene {existingGraphic.GetType().Name}. " +
                        "Se omite la asignación directa.",
                        target);
                    return;
                }

                image = target.gameObject.AddComponent<Image>();
            }

            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = preserveAspect;
            image.color = color;
        }

        private static Image CreateOrUpdateChildImage(
            Transform parent,
            string childName,
            Sprite sprite,
            Color color,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            if (parent == null)
            {
                return null;
            }

            Transform child = parent.Find(childName);
            GameObject childObject;

            if (child == null)
            {
                childObject = new GameObject(
                    childName,
                    typeof(RectTransform),
                    typeof(Image));
                childObject.transform.SetParent(parent, false);
            }
            else
            {
                childObject = child.gameObject;
            }

            RectTransform rect = childObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
            rect.localScale = Vector3.one;

            Image image = childObject.GetComponent<Image>();
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void ClearGeneratedLogo(Transform logoArea)
        {
            string[] generatedNames =
            {
                "UltimateLogo",
                "PuzzleLogo",
                "DinosaursLogo"
            };

            foreach (string generatedName in generatedNames)
            {
                Transform existing = logoArea.Find(generatedName);
                if (existing != null)
                {
                    Object.DestroyImmediate(existing.gameObject);
                }
            }
        }

        private static void CreateLogoImage(
            Transform parent,
            string name,
            Sprite sprite,
            Vector2 position,
            Vector2 size)
        {
            GameObject logoObject = new(
                name,
                typeof(RectTransform),
                typeof(Image));

            RectTransform rect =
                logoObject.GetComponent<RectTransform>();

            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = logoObject.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }
    }
}
#endif
