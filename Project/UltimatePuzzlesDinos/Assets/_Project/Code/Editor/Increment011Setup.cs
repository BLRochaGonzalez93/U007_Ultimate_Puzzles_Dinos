#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRMGames.UltimatePuzzlesDinos.Content;
using VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment011Setup
    {
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";
        private const string ConfigFolder = "Assets/_Project/Config/Puzzles";
        private const string DefinitionsFolder = ConfigFolder + "/Definitions";
        private const string CatalogPath = ConfigFolder + "/PuzzleCatalog.asset";
        private const string PuzzleSpritesFolder = "Assets/_Project/Art/Sprites/Puzzles";
        public static void Run()
        {
            if (!File.Exists(GameplayScenePath))
            {
                EditorUtility.DisplayDialog("Increment 011", "Run Increment 010 first.", "OK");
                return;
            }

            EnsureFolder("Assets/_Project/Config", "Puzzles");
            EnsureFolder(ConfigFolder, "Definitions");
            EnsureFolder("Assets/_Project/Art/Sprites", "Puzzles");

            PuzzleCatalog catalog = CreateOrUpdateCatalog();
            AssignCatalogToGameplayScene(catalog);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ultimate Puzzles Dinos] Increment 011 generated successfully.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 011 completed. Import puzzle sprites into:\n\n" + PuzzleSpritesFolder +
                "\n\nThen assign each sprite to its Puzzle Definition asset.",
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
            else
            {
                if (catalog.Count != 60) problems.Add($"Puzzle catalog contains {catalog.Count} entries instead of 60.");
                for (int level = 1; level <= 60; level++)
                {
                    string path = GetDefinitionPath(level);
                    PuzzleDefinition definition = AssetDatabase.LoadAssetAtPath<PuzzleDefinition>(path);
                    if (definition == null) problems.Add($"Missing definition: {path}");
                }
            }

            if (!File.Exists(GameplayScenePath))
            {
                problems.Add($"Missing scene: {GameplayScenePath}");
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Additive);
                PuzzleBoardController board = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<PuzzleBoardController>(true))
                    .FirstOrDefault();

                if (board == null)
                {
                    problems.Add("PuzzleBoardController is missing from Gameplay.");
                }
                else
                {
                    SerializedObject serializedBoard = new(board);
                    Object assignedCatalog = serializedBoard.FindProperty("puzzleCatalog").objectReferenceValue;
                    if (assignedCatalog != catalog) problems.Add("PuzzleCatalog is not assigned to PuzzleBoardController.");
                }

                EditorSceneManager.CloseScene(scene, true);
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 011 validation passed.");
                EditorUtility.DisplayDialog("Validation passed", "Increment 011 is correctly installed.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 011 validation failed:\n- " + report);
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

            List<PuzzleDefinition> definitions = new();
            for (int level = 1; level <= 60; level++)
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

        private static void AssignCatalogToGameplayScene(PuzzleCatalog catalog)
        {
            Scene scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
            PuzzleBoardController board = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<PuzzleBoardController>(true))
                .FirstOrDefault();

            if (board == null)
            {
                throw new MissingComponentException("PuzzleBoardController was not found in Gameplay.unity. Run Increment 010 first.");
            }

            SerializedObject serializedBoard = new(board);
            serializedBoard.FindProperty("puzzleCatalog").objectReferenceValue = catalog;
            serializedBoard.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(board);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static string GetDefinitionPath(int level)
        {
            return $"{DefinitionsFolder}/Puzzle_{level:00}.asset";
        }

        private static Color GetFallbackColor(int level)
        {
            float hue = Mathf.Repeat(0.04f + (level - 1) * 0.067f, 1f);
            return Color.HSVToRGB(hue, 0.68f, 0.82f);
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
