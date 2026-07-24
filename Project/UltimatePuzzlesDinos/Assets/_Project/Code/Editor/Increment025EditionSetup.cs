#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRMGames.UltimatePuzzlesDinos.Application;
using VRMGames.UltimatePuzzlesDinos.Configuration;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment025EditionSetup
    {
        private const string BootstrapScenePath =
            "Assets/_Project/Scenes/Bootstrap.unity";
        private const string FreeConfigPath =
            "Assets/_Project/Config/Editions/Edition_Free.asset";
        private const string PremiumConfigPath =
            "Assets/_Project/Config/Editions/Edition_Premium.asset";

        public static void Run()
        {
            EditionConfig free = LoadAndConfigure(
                FreeConfigPath,
                GameEdition.Free,
                true,
                true,
                false,
                3);

            EditionConfig premium = LoadAndConfigure(
                PremiumConfigPath,
                GameEdition.Premium,
                false,
                false,
                true,
                60);

            if (free == null || premium == null)
            {
                EditorUtility.DisplayDialog(
                    "Increment 025",
                    "No se pudieron cargar las configuraciones de edición.",
                    "OK");
                return;
            }

            AssignBootstrapConfig(free);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                "[Increment 025] Free and Premium edition profiles validated. " +
                "Bootstrap remains configured for Free development testing.");

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 025 completado.\n\n" +
                "Bootstrap se ha dejado en edición Free para las pruebas " +
                "habituales del flujo de anuncios.",
                "OK");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Edition/Use Free in Bootstrap")]
        public static void UseFree()
        {
            AssignBootstrapConfig(
                AssetDatabase.LoadAssetAtPath<EditionConfig>(FreeConfigPath));
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Edition/Use Premium in Bootstrap")]
        public static void UsePremium()
        {
            AssignBootstrapConfig(
                AssetDatabase.LoadAssetAtPath<EditionConfig>(PremiumConfigPath));
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Edition/Show Active Bootstrap Edition")]
        public static void ShowActiveEdition()
        {
            AppBootstrap bootstrap = LoadBootstrap();
            EditionConfig config = bootstrap != null
                ? new SerializedObject(bootstrap)
                    .FindProperty("editionConfig")
                    .objectReferenceValue as EditionConfig
                : null;

            string message = config != null
                ? $"Bootstrap usa: {config.name} ({config.Edition})"
                : "Bootstrap no tiene EditionConfig asignado.";

            Debug.Log("[Edition] " + message, config);
            EditorUtility.DisplayDialog("Edición activa", message, "OK");
        }

        public static void Validate()
        {
            List<string> problems = new();
            EditionConfig free =
                AssetDatabase.LoadAssetAtPath<EditionConfig>(FreeConfigPath);
            EditionConfig premium =
                AssetDatabase.LoadAssetAtPath<EditionConfig>(PremiumConfigPath);

            if (free == null)
            {
                problems.Add("Falta Edition_Free.asset.");
            }
            else
            {
                if (free.Edition != GameEdition.Free) problems.Add("Edition_Free no está marcada como Free.");
                if (!free.AdsEnabled) problems.Add("Edition_Free debe tener Ads Enabled.");
                if (!free.RewardedUnlocksEnabled) problems.Add("Edition_Free debe permitir Rewarded Unlocks.");
                if (free.AllContentUnlocked) problems.Add("Edition_Free no debe desbloquear todo el contenido.");
                if (free.InitiallyUnlockedPuzzleCount != 3) problems.Add("Edition_Free debe comenzar con 3 niveles.");
            }

            if (premium == null)
            {
                problems.Add("Falta Edition_Premium.asset.");
            }
            else
            {
                if (premium.Edition != GameEdition.Premium) problems.Add("Edition_Premium no está marcada como Premium.");
                if (premium.AdsEnabled) problems.Add("Edition_Premium no debe tener anuncios.");
                if (premium.RewardedUnlocksEnabled) problems.Add("Edition_Premium no debe permitir Rewarded Unlocks.");
                if (!premium.AllContentUnlocked) problems.Add("Edition_Premium debe desbloquear todo el contenido.");
                if (premium.InitiallyUnlockedPuzzleCount != 60) problems.Add("Edition_Premium debe contener 60 niveles iniciales.");
            }

            AppBootstrap bootstrap = LoadBootstrap();
            if (bootstrap == null)
            {
                problems.Add("Bootstrap.unity no contiene AppBootstrap.");
            }
            else
            {
                SerializedProperty property =
                    new SerializedObject(bootstrap).FindProperty("editionConfig");
                if (property == null || property.objectReferenceValue == null)
                {
                    problems.Add("AppBootstrap no tiene EditionConfig asignado.");
                }
            }

            string[] requiredScripts =
            {
                "Assets/_Project/Code/Runtime/Configuration/EditionService.cs",
                "Assets/_Project/Code/Runtime/Application/AppBootstrap.cs",
                "Assets/_Project/Code/Runtime/Gameplay/ProgressService.cs",
                "Assets/_Project/Code/Runtime/Monetization/RewardedUnlockService.cs"
            };

            problems.AddRange(requiredScripts.Where(path => !File.Exists(path)));

            if (problems.Count == 0)
            {
                Debug.Log("[Increment 025] Validación correcta.");
                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 025 instalado correctamente.",
                    "OK");
                return;
            }

            Debug.LogError(
                "[Increment 025] Validación fallida:\n- " +
                string.Join("\n- ", problems));
            EditorUtility.DisplayDialog(
                "Validación fallida",
                string.Join("\n", problems),
                "OK");
        }

        private static EditionConfig LoadAndConfigure(
            string path,
            GameEdition edition,
            bool ads,
            bool rewarded,
            bool allUnlocked,
            int initialLevels)
        {
            EditionConfig config =
                AssetDatabase.LoadAssetAtPath<EditionConfig>(path);
            if (config == null)
            {
                Debug.LogError($"[Increment 025] Missing {path}.");
                return null;
            }

            config.Configure(
                edition,
                ads,
                rewarded,
                allUnlocked,
                initialLevels);
            EditorUtility.SetDirty(config);
            return config;
        }

        private static void AssignBootstrapConfig(EditionConfig config)
        {
            if (config == null)
            {
                Debug.LogError("[Edition] Cannot assign a null EditionConfig.");
                return;
            }

            string configPath = AssetDatabase.GetAssetPath(config);

            if (string.IsNullOrEmpty(configPath))
            {
                Debug.LogError(
                    "[Edition] Cannot determine the EditionConfig asset path.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(
                BootstrapScenePath,
                OpenSceneMode.Single);

            // Recargar el asset después de abrir la escena.
            config = AssetDatabase.LoadAssetAtPath<EditionConfig>(configPath);

            if (config == null)
            {
                Debug.LogError(
                    $"[Edition] Could not reload EditionConfig at {configPath}.");
                return;
            }

            AppBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(root =>
                    root.GetComponentsInChildren<AppBootstrap>(true))
                .FirstOrDefault();

            if (bootstrap == null)
            {
                Debug.LogError(
                    "[Edition] AppBootstrap not found in Bootstrap.unity.");
                return;
            }

            SerializedObject serialized = new(bootstrap);
            SerializedProperty editionProperty =
                serialized.FindProperty("editionConfig");

            if (editionProperty == null)
            {
                Debug.LogError(
                    "[Edition] AppBootstrap does not contain editionConfig.");
                return;
            }

            editionProperty.objectReferenceValue = config;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            AssetDatabase.SaveAssets();

            Debug.Log(
                $"[Edition] Bootstrap now uses " +
                $"{config.name} ({config.Edition}).",
                config);
        }

        private static AppBootstrap LoadBootstrap()
        {
            if (!File.Exists(BootstrapScenePath))
            {
                return null;
            }

            Scene scene = EditorSceneManager.OpenScene(
                BootstrapScenePath,
                OpenSceneMode.Single);
            return scene.GetRootGameObjects()
                .SelectMany(root =>
                    root.GetComponentsInChildren<AppBootstrap>(true))
                .FirstOrDefault();
        }
    }
}
#endif
