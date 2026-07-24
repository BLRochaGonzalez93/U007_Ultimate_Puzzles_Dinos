#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Content;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment012Setup
    {
        private const string LevelSelectionScenePath = "Assets/_Project/Scenes/LevelSelection.unity";
        private const string ConfigFolder = "Assets/_Project/Config/Puzzles";
        private const string DefinitionsFolder = ConfigFolder + "/Definitions";
        private const string CatalogPath = ConfigFolder + "/PuzzleCatalog.asset";
        private const int LevelCount = PuzzleLevelCatalog.TotalLevelCount;
        private const int InitialUnlockedLevels = PuzzleLevelCatalog.InitialUnlockedLevels;

        private static readonly Color Wood = new(0.36f, 0.20f, 0.10f, 1f);
        private static readonly Color Locked = new(0.20f, 0.20f, 0.20f, 1f);
        private static readonly Color Accent = new(1f, 0.69f, 0.03f, 1f);
        private static readonly Color TextPrimary = new(1f, 0.97f, 0.88f, 1f);
        private static readonly Color TextMuted = new(0.72f, 0.68f, 0.60f, 1f);
        public static void Run()
        {
            if (!File.Exists(LevelSelectionScenePath))
            {
                EditorUtility.DisplayDialog("Increment 012", "Run Increment 005 first.", "OK");
                return;
            }

            EnsureFolder("Assets/_Project/Config", "Puzzles");
            EnsureFolder(ConfigFolder, "Definitions");

            PuzzleCatalog catalog = CreateOrUpdateCatalog();
            UpdateLevelSelectionScene(catalog);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ultimate Puzzles Dinos] Increment 012 generated successfully. Level count: 60.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 012 completed. The catalog and level-selection UI now contain 60 levels.",
                "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            PuzzleCatalog catalog = AssetDatabase.LoadAssetAtPath<PuzzleCatalog>(CatalogPath);

            if (catalog == null)
            {
                problems.Add($"Missing catalog: {CatalogPath}");
            }
            else if (catalog.Count != LevelCount)
            {
                problems.Add($"Puzzle catalog contains {catalog.Count} entries instead of {LevelCount}.");
            }

            for (int level = 1; level <= LevelCount; level++)
            {
                string path = GetDefinitionPath(level);
                if (AssetDatabase.LoadAssetAtPath<PuzzleDefinition>(path) == null)
                {
                    problems.Add($"Missing definition: {path}");
                }
            }

            if (!File.Exists(LevelSelectionScenePath))
            {
                problems.Add($"Missing scene: {LevelSelectionScenePath}");
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(LevelSelectionScenePath, OpenSceneMode.Additive);
                LevelSelectionScreen screen = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<LevelSelectionScreen>(true))
                    .FirstOrDefault();

                RectTransform content = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<RectTransform>(true))
                    .FirstOrDefault(rect => rect.name == "Content" && rect.parent != null && rect.parent.name == "Viewport");

                if (screen == null)
                {
                    problems.Add("LevelSelectionScreen is missing from LevelSelection.unity.");
                }
                else
                {
                    SerializedObject serializedScreen = new(screen);
                    if (serializedScreen.FindProperty("levelButtons").arraySize != LevelCount)
                    {
                        problems.Add("LevelSelectionScreen does not reference 60 level buttons.");
                    }

                    if (serializedScreen.FindProperty("puzzleCatalog").objectReferenceValue != catalog)
                    {
                        problems.Add("PuzzleCatalog is not assigned to LevelSelectionScreen.");
                    }
                }

                if (content == null || content.childCount != LevelCount)
                {
                    problems.Add($"Level-selection content contains {content?.childCount ?? 0} cards instead of {LevelCount}.");
                }

                EditorSceneManager.CloseScene(scene, true);
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 012 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 012 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 012 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }

        private static PuzzleCatalog CreateOrUpdateCatalog()
        {
            PuzzleCatalog catalog = AssetDatabase.LoadAssetAtPath<PuzzleCatalog>(CatalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<PuzzleCatalog>();
                AssetDatabase.CreateAsset(catalog, CatalogPath);
            }

            List<PuzzleDefinition> definitions = new(LevelCount);
            for (int level = 1; level <= LevelCount; level++)
            {
                definitions.Add(CreateOrUpdateDefinition(level));
            }

            SerializedObject serializedCatalog = new(catalog);
            SerializedProperty puzzles = serializedCatalog.FindProperty("puzzles");
            puzzles.arraySize = definitions.Count;
            for (int index = 0; index < definitions.Count; index++)
            {
                puzzles.GetArrayElementAtIndex(index).objectReferenceValue = definitions[index];
            }

            serializedCatalog.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static PuzzleDefinition CreateOrUpdateDefinition(int level)
        {
            string path = GetDefinitionPath(level);
            PuzzleDefinition definition = AssetDatabase.LoadAssetAtPath<PuzzleDefinition>(path);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<PuzzleDefinition>();
                AssetDatabase.CreateAsset(definition, path);
            }

            SerializedObject serializedDefinition = new(definition);
            serializedDefinition.FindProperty("id").stringValue = $"dino_{level:000}";
            serializedDefinition.FindProperty("displayName").stringValue = $"Dinosaurio {level:00}";
            serializedDefinition.FindProperty("fallbackColor").colorValue = GetFallbackColor(level);
            serializedDefinition.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static void UpdateLevelSelectionScene(PuzzleCatalog catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(LevelSelectionScenePath, OpenSceneMode.Single);
            LevelSelectionScreen screen = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<LevelSelectionScreen>(true))
                .FirstOrDefault();

            RectTransform content = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<RectTransform>(true))
                .FirstOrDefault(rect => rect.name == "Content" && rect.parent != null && rect.parent.name == "Viewport");

            if (screen == null || content == null)
            {
                throw new MissingReferenceException("LevelSelection.unity does not contain the expected LevelSelectionScreen/Viewport/Content structure.");
            }

            while (content.childCount > 0)
            {
                Object.DestroyImmediate(content.GetChild(0).gameObject);
            }

            content.sizeDelta = new Vector2(LevelCount * 275f + (LevelCount - 1) * 28f + 60f, 610f);

            List<Button> buttons = new(LevelCount);
            List<Text> labels = new(LevelCount);
            List<Image> previews = new(LevelCount);
            List<Text> placeholders = new(LevelCount);
            List<GameObject> locks = new(LevelCount);

            for (int index = 0; index < LevelCount; index++)
            {
                int level = index + 1;
                bool unlocked = level <= InitialUnlockedLevels;
                PuzzleDefinition definition = catalog.GetByLevelNumber(level);

                RectTransform card = CreateRect($"LevelCard_{level:00}", content);
                card.sizeDelta = new Vector2(275f, 560f);
                Image cardImage = card.gameObject.AddComponent<Image>();
                cardImage.color = unlocked ? Wood : Locked;
                Button button = card.gameObject.AddComponent<Button>();
                button.targetGraphic = cardImage;

                RectTransform previewRect = CreateRect("Preview", card);
                previewRect.anchorMin = new Vector2(0.5f, 1f);
                previewRect.anchorMax = new Vector2(0.5f, 1f);
                previewRect.pivot = new Vector2(0.5f, 1f);
                previewRect.anchoredPosition = new Vector2(0f, -24f);
                previewRect.sizeDelta = new Vector2(225f, 390f);
                Image previewImage = previewRect.gameObject.AddComponent<Image>();
                previewImage.sprite = definition != null ? definition.Image : null;
                previewImage.preserveAspect = true;
                previewImage.color = previewImage.sprite != null
                    ? Color.white
                    : unlocked ? GetFallbackColor(level) : new Color(0.11f, 0.11f, 0.11f, 1f);

                Text placeholder = CreateText(
                    "PreviewPlaceholder",
                    previewRect,
                    unlocked ? $"DINOSAURIO\n{level:00}" : "?",
                    32,
                    unlocked ? TextMuted : TextPrimary,
                    FontStyle.Bold);
                Stretch(placeholder.rectTransform, 18f);
                placeholder.gameObject.SetActive(previewImage.sprite == null);

                Text label = CreateText("LevelLabel", card, $"NIVEL {level:00}", 30, TextPrimary, FontStyle.Bold);
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

                buttons.Add(button);
                labels.Add(label);
                previews.Add(previewImage);
                placeholders.Add(placeholder);
                locks.Add(lockText.gameObject);
            }

            SerializedObject serialized = new(screen);
            serialized.FindProperty("puzzleCatalog").objectReferenceValue = catalog;
            SetObjectList(serialized.FindProperty("levelButtons"), buttons.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("levelLabels"), labels.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("previewImages"), previews.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("previewPlaceholders"), placeholders.Cast<Object>().ToList());
            SetObjectList(serialized.FindProperty("lockIndicators"), locks.Cast<Object>().ToList());
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(screen);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static string GetDefinitionPath(int level) => $"{DefinitionsFolder}/Puzzle_{level:00}.asset";

        private static Color GetFallbackColor(int level)
        {
            float hue = Mathf.Repeat(0.04f + (level - 1) * 0.067f, 1f);
            return Color.HSVToRGB(hue, 0.68f, 0.82f);
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject gameObject = new(name, typeof(RectTransform));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
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

        private static void Stretch(RectTransform rect, float margin = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(margin, margin);
            rect.offsetMax = new Vector2(-margin, -margin);
        }

        private static void SetObjectList(SerializedProperty property, List<Object> objects)
        {
            property.arraySize = objects.Count;
            for (int index = 0; index < objects.Count; index++)
            {
                property.GetArrayElementAtIndex(index).objectReferenceValue = objects[index];
            }
        }

        private static void EnsureFolder(string parent, string child)
        {
            string fullPath = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
#endif
