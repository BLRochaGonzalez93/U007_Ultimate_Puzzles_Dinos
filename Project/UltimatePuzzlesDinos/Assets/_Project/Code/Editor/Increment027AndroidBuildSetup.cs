#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Configuration;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment027AndroidBuildSetup
    {
        private const string ProfilesFolder = "Assets/_Project/Config/BuildProfiles";
        private const string FreeEditionPath = "Assets/_Project/Config/Editions/Edition_Free.asset";
        private const string PremiumEditionPath = "Assets/_Project/Config/Editions/Edition_Premium.asset";

        public static void Run()
        {
            EnsureFolder("Assets/_Project/Config");
            EnsureFolder(ProfilesFolder);

            EditionConfig free = AssetDatabase.LoadAssetAtPath<EditionConfig>(FreeEditionPath);
            EditionConfig premium = AssetDatabase.LoadAssetAtPath<EditionConfig>(PremiumEditionPath);
            if (free == null || premium == null)
            {
                Debug.LogError("[Increment 027] Edition assets are missing.");
                return;
            }

            CreateOrUpdate(
                "Android_Free_Development.asset",
                AndroidBuildFlavor.FreeDevelopment,
                free,
                "Ultimate Puzzles Dinos Free DEV",
                "com.vrmgames.ultimatepuzzlesdinos.free.dev",
                true,
                true);

            CreateOrUpdate(
                "Android_Free_Release.asset",
                AndroidBuildFlavor.FreeRelease,
                free,
                "Ultimate Puzzles Dinos",
                "com.vrmgames.ultimatepuzzlesdinos.free",
                false,
                false);

            CreateOrUpdate(
                "Android_Premium_Development.asset",
                AndroidBuildFlavor.PremiumDevelopment,
                premium,
                "Ultimate Puzzles Dinos Premium DEV",
                "com.vrmgames.ultimatepuzzlesdinos.premium.dev",
                true,
                true);

            CreateOrUpdate(
                "Android_Premium_Release.asset",
                AndroidBuildFlavor.PremiumRelease,
                premium,
                "Ultimate Puzzles Dinos Premium",
                "com.vrmgames.ultimatepuzzlesdinos.premium",
                false,
                false);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = Load(AndroidBuildFlavor.FreeDevelopment);

            Debug.Log("[Increment 027] Android build profiles created.");
            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 027 completado.\n\n" +
                "Se han creado cuatro perfiles Android.",
                "OK");
        }

        public static void Validate()
        {
            List<string> problems = new();
            foreach (AndroidBuildFlavor flavor in System.Enum.GetValues(typeof(AndroidBuildFlavor)))
            {
                AndroidBuildProfile profile = Load(flavor);
                if (profile == null)
                {
                    problems.Add($"Falta el perfil {flavor}.");
                    continue;
                }
                problems.AddRange(AndroidBuildProfileService.ValidateProfile(profile));
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Increment 027] Validation passed.");
                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Los cuatro perfiles Android son válidos.",
                    "OK");
                return;
            }

            Debug.LogError("[Increment 027] Validation failed:\n- " + string.Join("\n- ", problems));
            EditorUtility.DisplayDialog(
                "Validación fallida",
                string.Join("\n", problems),
                "OK");
        }

        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Apply Free Development")]
        public static void ApplyFreeDevelopment() => Apply(AndroidBuildFlavor.FreeDevelopment);
        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Apply Free Release")]
        public static void ApplyFreeRelease() => Apply(AndroidBuildFlavor.FreeRelease);
        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Apply Premium Development")]
        public static void ApplyPremiumDevelopment() => Apply(AndroidBuildFlavor.PremiumDevelopment);
        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Apply Premium Release")]
        public static void ApplyPremiumRelease() => Apply(AndroidBuildFlavor.PremiumRelease);

        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Build Free Development")]
        public static void BuildFreeDevelopment() => Build(AndroidBuildFlavor.FreeDevelopment);
        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Build Free Release")]
        public static void BuildFreeRelease() => Build(AndroidBuildFlavor.FreeRelease);
        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Build Premium Development")]
        public static void BuildPremiumDevelopment() => Build(AndroidBuildFlavor.PremiumDevelopment);
        [MenuItem("VRM Games/Ultimate Puzzles Dinos/Android Build/Build Premium Release")]
        public static void BuildPremiumRelease() => Build(AndroidBuildFlavor.PremiumRelease);

        private static void Apply(AndroidBuildFlavor flavor)
        {
            AndroidBuildProfileService.Apply(Load(flavor));
        }

        private static void Build(AndroidBuildFlavor flavor)
        {
            AndroidBuildProfileService.Build(Load(flavor));
        }

        private static AndroidBuildProfile Load(AndroidBuildFlavor flavor)
        {
            string name = flavor switch
            {
                AndroidBuildFlavor.FreeDevelopment => "Android_Free_Development.asset",
                AndroidBuildFlavor.FreeRelease => "Android_Free_Release.asset",
                AndroidBuildFlavor.PremiumDevelopment => "Android_Premium_Development.asset",
                _ => "Android_Premium_Release.asset"
            };
            return AssetDatabase.LoadAssetAtPath<AndroidBuildProfile>($"{ProfilesFolder}/{name}");
        }

        private static void CreateOrUpdate(
            string fileName,
            AndroidBuildFlavor flavor,
            EditionConfig edition,
            string productName,
            string identifier,
            bool development,
            bool debugging)
        {
            string path = $"{ProfilesFolder}/{fileName}";
            AndroidBuildProfile profile = AssetDatabase.LoadAssetAtPath<AndroidBuildProfile>(path);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<AndroidBuildProfile>();
                AssetDatabase.CreateAsset(profile, path);
            }

            SerializedObject serialized = new(profile);
            serialized.FindProperty("flavor").enumValueIndex = (int)flavor;
            serialized.FindProperty("editionConfig").objectReferenceValue = edition;
            serialized.FindProperty("productName").stringValue = productName;
            serialized.FindProperty("applicationIdentifier").stringValue = identifier;
            serialized.FindProperty("bundleVersion").stringValue = "1.0.0";
            serialized.FindProperty("bundleVersionCode").intValue = 1;
            serialized.FindProperty("developmentBuild").boolValue = development;
            serialized.FindProperty("allowDebugging").boolValue = debugging;
            serialized.FindProperty("buildAppBundle").boolValue = true;
            serialized.FindProperty("outputFolder").stringValue = "Builds/Android";
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(profile);
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            string parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
            string name = Path.GetFileName(folder);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
