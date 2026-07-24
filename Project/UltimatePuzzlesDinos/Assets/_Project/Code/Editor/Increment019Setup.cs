#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment019Setup
    {
        private const string GameplayScenePath =
            "Assets/_Project/Scenes/Gameplay.unity";
        public static void Run()
        {
            if (!File.Exists(GameplayScenePath))
            {
                EditorUtility.DisplayDialog(
                    "Increment 019",
                    "No se ha encontrado Gameplay.unity.",
                    "OK");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(
                GameplayScenePath,
                OpenSceneMode.Single);

            PuzzleBoardController standardBoard =
                scene.GetRootGameObjects()
                    .SelectMany(root =>
                        root.GetComponentsInChildren<PuzzleBoardController>(true))
                    .FirstOrDefault();

            GameplayScreen gameplayScreen =
                scene.GetRootGameObjects()
                    .SelectMany(root =>
                        root.GetComponentsInChildren<GameplayScreen>(true))
                    .FirstOrDefault();

            if (standardBoard == null || gameplayScreen == null)
            {
                EditorUtility.DisplayDialog(
                    "Increment 019",
                    "Gameplay no contiene los componentes requeridos.",
                    "OK");
                return;
            }

            PuzzleLogicBoardController logicBoard =
                standardBoard.GetComponent<PuzzleLogicBoardController>();

            if (logicBoard == null)
            {
                logicBoard =
                    standardBoard.gameObject
                        .AddComponent<PuzzleLogicBoardController>();
            }

            SerializedObject logicSerialized = new(logicBoard);
            logicSerialized.FindProperty("layoutSource").objectReferenceValue =
                standardBoard;
            logicSerialized.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject screenSerialized = new(gameplayScreen);
            screenSerialized.FindProperty("logicBoardController")
                .objectReferenceValue = logicBoard;
            screenSerialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(logicBoard);
            EditorUtility.SetDirty(gameplayScreen);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "[Ultimate Puzzles Dinos] Increment 019 installed. " +
                "Puzzle Logic is now playable.");

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 019 completado.\n\n" +
                "El modo Puzzle Logic ya es jugable.",
                "OK");
        }
        public static void Validate()
        {
            List<string> problems = new();

            string[] scripts =
            {
                "Assets/_Project/Code/Runtime/Gameplay/Puzzle/" +
                "PuzzleLogicBoardController.cs",
                "Assets/_Project/Code/Runtime/UI/Screens/" +
                "GameplayScreen.cs"
            };

            foreach (string script in scripts)
            {
                if (!File.Exists(script))
                {
                    problems.Add($"Falta: {script}");
                }
            }

            if (!File.Exists(GameplayScenePath))
            {
                problems.Add($"Falta: {GameplayScenePath}");
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(
                    GameplayScenePath,
                    OpenSceneMode.Additive);

                GameObject[] roots = scene.GetRootGameObjects();

                if (!roots.Any(root =>
                    root.GetComponentInChildren
                        <PuzzleLogicBoardController>(true) != null))
                {
                    problems.Add(
                        "Gameplay no contiene PuzzleLogicBoardController.");
                }

                GameplayScreen screen = roots
                    .SelectMany(root =>
                        root.GetComponentsInChildren<GameplayScreen>(true))
                    .FirstOrDefault();

                if (screen == null)
                {
                    problems.Add("GameplayScreen no existe.");
                }

                EditorSceneManager.CloseScene(scene, true);
            }

            if (problems.Count == 0)
            {
                Debug.Log(
                    "[Ultimate Puzzles Dinos] " +
                    "Increment 019 validation passed.");

                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 019 instalado correctamente.",
                    "OK");
                return;
            }

            string report = string.Join("\n- ", problems);
            Debug.LogError(
                "[Ultimate Puzzles Dinos] " +
                "Increment 019 validation failed:\n- " + report);

            EditorUtility.DisplayDialog(
                "Validación fallida",
                "Problemas encontrados:\n\n- " + report,
                "OK");
        }
    }
}
#endif
