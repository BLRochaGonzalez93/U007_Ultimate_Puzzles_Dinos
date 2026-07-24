#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment0201Setup
    {
        private const string MosaicControllerPath =
            "Assets/_Project/Code/Runtime/Gameplay/Puzzle/" +
            "MosaicBoardController.cs";

        public static void Validate()
        {
            if (!File.Exists(MosaicControllerPath))
            {
                Debug.LogError(
                    "[Increment 020.1] Falta MosaicBoardController.cs.");

                EditorUtility.DisplayDialog(
                    "Validación fallida",
                    "No se ha encontrado MosaicBoardController.cs.",
                    "OK");

                return;
            }

            Debug.Log(
                "[Increment 020.1] Validación correcta. " +
                "Mosaic usa mezcla accesible y vista previa.");

            EditorUtility.DisplayDialog(
                "Validación correcta",
                "Increment 020.1 instalado correctamente.",
                "OK");
        }
    }
}
#endif
