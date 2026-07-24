#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.UI;
using VRMGames.UltimatePuzzlesDinos.UI.Responsive;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment023Setup
    {
        private static readonly string[] ScenePaths =
        {
            "Assets/_Project/Scenes/MainMenu.unity",
            "Assets/_Project/Scenes/LevelSelection.unity",
            "Assets/_Project/Scenes/DifficultySelection.unity",
            "Assets/_Project/Scenes/Gameplay.unity"
        };

        public static void Run()
        {
            List<string> missing = ScenePaths.Where(path => !File.Exists(path)).ToList();
            if (missing.Count > 0)
            {
                EditorUtility.DisplayDialog(
                    "Increment 023",
                    "Faltan escenas:\n\n" + string.Join("\n", missing),
                    "OK");
                return;
            }

            foreach (string scenePath in ScenePaths)
            {
                PatchScene(scenePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Increment 023] UI responsive instalada.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 023 completado.\n\nUI responsive y Safe Area configuradas.",
                "OK");
        }

        public static void Validate()
        {
            List<string> problems = new();

            foreach (string path in ScenePaths)
            {
                if (!File.Exists(path))
                {
                    problems.Add($"Falta: {path}");
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                Canvas canvas = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<Canvas>(true))
                    .FirstOrDefault();

                if (canvas == null)
                {
                    problems.Add($"{path}: no contiene Canvas");
                }
                else
                {
                    if (canvas.GetComponent<CanvasScaler>() == null)
                    {
                        problems.Add($"{path}: falta CanvasScaler");
                    }
                    if (canvas.GetComponent<ResponsiveCanvasController>() == null)
                    {
                        problems.Add($"{path}: falta ResponsiveCanvasController");
                    }
                    if (canvas.GetComponentInChildren<ResponsiveSceneLayout>(true) == null)
                    {
                        problems.Add($"{path}: falta ResponsiveSceneLayout");
                    }
                }

                EditorSceneManager.CloseScene(scene, true);
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Increment 023] Validación correcta.");
                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 023 instalado correctamente.",
                    "OK");
                return;
            }

            Debug.LogError("[Increment 023] Problemas:\n- " + string.Join("\n- ", problems));
            EditorUtility.DisplayDialog(
                "Validación fallida",
                string.Join("\n", problems),
                "OK");
        }

        public static void PreviewPhone169() => SetGameViewSize(1080, 1920, "Phone 16x9");

        public static void PreviewPhone1959() => SetGameViewSize(1080, 2340, "Phone 19.5x9");

        public static void PreviewPhone209() => SetGameViewSize(1080, 2400, "Phone 20x9");

        public static void PreviewTablet43() => SetGameViewSize(1536, 2048, "Tablet 4x3");

        private static void PatchScene(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            Canvas canvas = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Canvas>(true))
                .FirstOrDefault();

            if (canvas == null)
            {
                Debug.LogWarning($"[Increment 023] Sin Canvas: {scenePath}");
                return;
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            EnsureComponent<ResponsiveCanvasController>(canvas.transform);

            Transform safeArea = Find(canvas.transform, "SafeArea");
            if (safeArea == null)
            {
                GameObject go = new("SafeArea", typeof(RectTransform));
                safeArea = go.transform;
                safeArea.SetParent(canvas.transform, false);

                RectTransform rect = safeArea as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                List<Transform> children = new();
                foreach (Transform child in canvas.transform)
                {
                    if (child != safeArea)
                    {
                        children.Add(child);
                    }
                }

                foreach (Transform child in children)
                {
                    child.SetParent(safeArea, true);
                }
            }

            EnsureComponent<SafeAreaFitter>(safeArea);
            ResponsiveSceneLayout layout = EnsureComponent<ResponsiveSceneLayout>(safeArea);
            SerializedObject serialized = new(layout);
            SerializedProperty property = serialized.FindProperty("safeAreaRoot");
            if (property != null)
            {
                property.objectReferenceValue = safeArea as RectTransform;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(canvas);
            EditorUtility.SetDirty(layout);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
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

        private static void SetGameViewSize(int width, int height, string label)
        {
            Debug.Log($"[Responsive Preview] {label}: {width}x{height}. " +
                "Selecciona esta resolución en Game View si no aparece automáticamente.");

            EditorUtility.DisplayDialog(
                "Responsive Preview",
                $"Perfil recomendado: {label}\nResolución: {width} × {height}\n\n" +
                "Usa la lista de resolución de la ventana Game para seleccionarla.",
                "OK");
        }
    }
}
#endif
