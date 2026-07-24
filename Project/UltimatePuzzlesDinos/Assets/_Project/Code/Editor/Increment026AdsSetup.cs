#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Monetization;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment026AdsSetup
    {
        private const string Folder = "Assets/_Project/Resources/Monetization";
        private const string PolicyPath = Folder + "/AdsPolicy.asset";

        public static void Run()
        {
            EnsureFolder("Assets/_Project/Resources");
            EnsureFolder(Folder);

            AdsPolicy policy = AssetDatabase.LoadAssetAtPath<AdsPolicy>(PolicyPath);
            if (policy == null)
            {
                policy = ScriptableObject.CreateInstance<AdsPolicy>();
                AssetDatabase.CreateAsset(policy, PolicyPath);
            }

            EditorUtility.SetDirty(policy);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = policy;

            Debug.Log("[Increment 026] Ads architecture installed.", policy);
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 026 completado.\n\n" +
                "Se ha creado AdsPolicy y activado el proveedor simulado.",
                "OK");
        }

        public static void Validate()
        {
            List<string> problems = new();
            string[] scripts =
            {
                "Assets/_Project/Code/Runtime/Monetization/AdPlacement.cs",
                "Assets/_Project/Code/Runtime/Monetization/IAdsProvider.cs",
                "Assets/_Project/Code/Runtime/Monetization/AdsPolicy.cs",
                "Assets/_Project/Code/Runtime/Monetization/MockAdsProvider.cs",
                "Assets/_Project/Code/Runtime/Monetization/AdsService.cs",
                "Assets/_Project/Code/Runtime/Monetization/RewardedUnlockService.cs"
            };

            foreach (string script in scripts)
            {
                if (!File.Exists(script))
                {
                    problems.Add($"Falta {script}");
                }
            }

            if (AssetDatabase.LoadAssetAtPath<AdsPolicy>(PolicyPath) == null)
            {
                problems.Add($"Falta {PolicyPath}");
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Increment 026] Validation passed.");
                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 026 instalado correctamente.",
                    "OK");
                return;
            }

            Debug.LogError(
                "[Increment 026] Validation failed:\n- " +
                string.Join("\n- ", problems));

            EditorUtility.DisplayDialog(
                "Validación fallida",
                string.Join("\n", problems),
                "OK");
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
