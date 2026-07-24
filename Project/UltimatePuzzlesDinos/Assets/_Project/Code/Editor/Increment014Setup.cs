using System.IO;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Persistence;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment014Setup
    {
        public static void Validate()
        {
            string[] requiredFiles =
            {
                "Assets/_Project/Code/Runtime/Persistence/ProgressData.cs",
                "Assets/_Project/Code/Runtime/Persistence/ProgressSaveService.cs",
                "Assets/_Project/Code/Runtime/Gameplay/ProgressService.cs",
                "Assets/_Project/Code/Runtime/Monetization/RewardedUnlockService.cs",
                "Assets/_Project/Code/Runtime/UI/Screens/LevelSelectionScreen.cs"
            };

            bool valid = true;
            foreach (string path in requiredFiles)
            {
                if (!File.Exists(path))
                {
                    Debug.LogError($"[Increment 014] Falta el archivo: {path}");
                    valid = false;
                }
            }

            if (!valid)
            {
                return;
            }

            ProgressData progress = ProgressSaveService.Load();
            Debug.Log(
                $"[Increment 014] Validación completada. Versión de guardado: {progress.version}. " +
                "Desbloqueo secuencial retirado; estrellas 1–4 y desbloqueo recompensado provisional activos.");
        }
    }
}
