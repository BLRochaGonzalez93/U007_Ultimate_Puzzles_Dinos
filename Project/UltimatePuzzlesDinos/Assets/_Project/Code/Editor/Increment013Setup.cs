using System.IO;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Content;
using VRMGames.UltimatePuzzlesDinos.Persistence;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment013Setup
    {
        private const string CatalogPath = "Assets/_Project/Config/Puzzles/PuzzleCatalog.asset";
        public static void Validate()
        {
            PuzzleCatalog catalog = AssetDatabase.LoadAssetAtPath<PuzzleCatalog>(CatalogPath);
            if (catalog == null)
            {
                Debug.LogError($"[Increment 013] No se encontró el catálogo en {CatalogPath}.");
                return;
            }

            int assignedImages = 0;
            for (int level = 1; level <= 60; level++)
            {
                PuzzleDefinition definition = catalog.GetByLevelNumber(level);
                if (definition != null && definition.Image != null)
                {
                    assignedImages++;
                }
            }

            Debug.Log($"[Increment 013] Validación completada. Catálogo: 60 niveles. Imágenes asignadas: {assignedImages}/60. El progreso se guardará en: {ProgressSaveService.SavePath}");
        }

        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Development/Reset Local Progress")]
        public static void ResetLocalProgress()
        {
            if (!EditorUtility.DisplayDialog(
                    "Restablecer progreso",
                    "Se eliminará el progreso local de Ultimate Puzzles Dinos en este equipo.",
                    "Restablecer",
                    "Cancelar"))
            {
                return;
            }

            ProgressSaveService.Delete();
            Debug.Log($"[Increment 013] Progreso local eliminado: {Path.GetDirectoryName(ProgressSaveService.SavePath)}");
        }
    }
}
