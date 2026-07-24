using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle;
using VRMGames.UltimatePuzzlesDinos.UI.Screens;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment015Setup
    {
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";
        public static void Validate()
        {
            bool valid = true;
            valid &= ValidateScript("Assets/_Project/Code/Runtime/Gameplay/Puzzle/PuzzleCompletionResult.cs");
            valid &= ValidateScript("Assets/_Project/Code/Runtime/UI/Screens/PuzzleResultPanel.cs");

            if (!File.Exists(GameplayScenePath))
            {
                Debug.LogError($"[Increment 015] Missing scene: {GameplayScenePath}");
                valid = false;
            }
            else
            {
                var previousScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
                var scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
                if (Object.FindFirstObjectByType<GameplayScreen>() == null)
                {
                    Debug.LogError("[Increment 015] GameplayScreen was not found in Gameplay scene.");
                    valid = false;
                }

                if (Object.FindFirstObjectByType<PuzzleBoardController>() == null)
                {
                    Debug.LogError("[Increment 015] PuzzleBoardController was not found in Gameplay scene.");
                    valid = false;
                }

                if (!string.IsNullOrEmpty(previousScene) && previousScene != scene.path && File.Exists(previousScene))
                {
                    EditorSceneManager.OpenScene(previousScene, OpenSceneMode.Single);
                }
            }

            if (valid)
            {
                Debug.Log("[Increment 015] Validation completed successfully. Result panel will be generated at runtime.");
            }
        }

        private static bool ValidateScript(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }

            Debug.LogError($"[Increment 015] Missing script: {path}");
            return false;
        }
    }
}
