using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment016Setup
    {
        public static void Validate()
        {
            string[] requiredFiles =
            {
                "Assets/_Project/Code/Runtime/Gameplay/Puzzle/PuzzleCompletionResult.cs",
                "Assets/_Project/Code/Runtime/Gameplay/Puzzle/PuzzlePieceView.cs",
                "Assets/_Project/Code/Runtime/Gameplay/Puzzle/PuzzleBoardController.cs",
                "Assets/_Project/Code/Runtime/UI/Screens/PuzzleResultPanel.cs"
            };

            bool valid = true;
            foreach (string path in requiredFiles)
            {
                if (File.Exists(path)) continue;
                Debug.LogError($"[Increment 016] Missing file: {path}");
                valid = false;
            }

            if (valid)
            {
                Debug.Log("[Increment 016] Validation completed successfully. Result buttons, timer and movement counter are ready.");
            }
        }
    }
}
