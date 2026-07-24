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

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment006InputSystemSetup
    {
        private const string ScenesFolder = "Assets/_Project/Scenes";
        public static void Run()
        {
            string activeScenePath = SceneManager.GetActiveScene().path;
            string[] scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .ToArray();

            if (scenePaths.Length == 0)
            {
                EditorUtility.DisplayDialog("Increment 006", $"No scenes were found under {ScenesFolder}.", "OK");
                return;
            }

            int migratedScenes = 0;
            int removedLegacyModules = 0;

            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                bool changed = false;

                StandaloneInputModule[] legacyModules = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<StandaloneInputModule>(true))
                    .ToArray();

                foreach (StandaloneInputModule legacyModule in legacyModules)
                {
                    Object.DestroyImmediate(legacyModule);
                    removedLegacyModules++;
                    changed = true;
                }

                InputSystemUIInputModule inputModule = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<InputSystemUIInputModule>(true))
                    .FirstOrDefault();

                EventSystem eventSystem = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<EventSystem>(true))
                    .FirstOrDefault();

                if (eventSystem == null)
                {
                    GameObject eventSystemObject = new("EventSystem", typeof(EventSystem));
                    SceneManager.MoveGameObjectToScene(eventSystemObject, scene);
                    eventSystem = eventSystemObject.GetComponent<EventSystem>();
                    changed = true;
                }

                if (inputModule == null)
                {
                    inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                    InputSystemUIActionsUtility.Configure(inputModule);
                    changed = true;
                }
                else
                {
                    InputSystemUIActionsUtility.Configure(inputModule);
                    changed = true;
                }

                if (changed)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                    migratedScenes++;
                }
            }

            if (!string.IsNullOrWhiteSpace(activeScenePath) && File.Exists(activeScenePath))
            {
                EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Ultimate Puzzles Dinos] Increment 006 completed. Migrated scenes: {migratedScenes}. Removed StandaloneInputModule components: {removedLegacyModules}.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                $"Input System UI migration completed.\n\nMigrated scenes: {migratedScenes}\nLegacy modules removed: {removedLegacyModules}",
                "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();
            string[] scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .ToArray();

            foreach (string scenePath in scenePaths)
            {
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                int legacyCount = scene.GetRootGameObjects()
                    .Sum(root => root.GetComponentsInChildren<StandaloneInputModule>(true).Length);
                int inputSystemCount = scene.GetRootGameObjects()
                    .Sum(root => root.GetComponentsInChildren<InputSystemUIInputModule>(true).Length);
                int eventSystemCount = scene.GetRootGameObjects()
                    .Sum(root => root.GetComponentsInChildren<EventSystem>(true).Length);

                if (legacyCount > 0)
                {
                    problems.Add($"{sceneName}: contains {legacyCount} StandaloneInputModule component(s).");
                }

                if (eventSystemCount > 0 && inputSystemCount == 0)
                {
                    problems.Add($"{sceneName}: EventSystem has no InputSystemUIInputModule.");
                }

                if (inputSystemCount > 1)
                {
                    problems.Add($"{sceneName}: contains more than one InputSystemUIInputModule.");
                }

                InputSystemUIInputModule[] modules = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<InputSystemUIInputModule>(true))
                    .ToArray();
                foreach (InputSystemUIInputModule module in modules)
                {
                    if (module.actionsAsset == null)
                    {
                        problems.Add($"{sceneName}: InputSystemUIInputModule has no persistent actions asset.");
                    }
                }

                EditorSceneManager.CloseScene(scene, true);
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Ultimate Puzzles Dinos] Increment 006 validation passed. All UI scenes use the new Input System.");
                EditorUtility.DisplayDialog("Validation passed", "All EventSystems use InputSystemUIInputModule.", "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError("[Ultimate Puzzles Dinos] Increment 006 validation failed:\n- " + report);
            EditorUtility.DisplayDialog("Validation failed", "Problems found:\n\n- " + report, "OK");
        }
    }
}
#endif
