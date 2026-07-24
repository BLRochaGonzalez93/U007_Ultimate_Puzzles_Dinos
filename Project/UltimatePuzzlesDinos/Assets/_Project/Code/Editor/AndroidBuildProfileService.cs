#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRMGames.UltimatePuzzlesDinos.Application;
using VRMGames.UltimatePuzzlesDinos.Configuration;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class AndroidBuildProfileService
    {
        private const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";

        public static bool Apply(AndroidBuildProfile profile, bool showDialog = true)
        {
            if (profile == null)
            {
                Debug.LogError("[Android Build] Profile is null.");
                return false;
            }

            List<string> problems = ValidateProfile(profile);
            if (problems.Count > 0)
            {
                ReportProblems(profile, problems, showDialog);
                return false;
            }

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                bool switched = EditorUserBuildSettings.SwitchActiveBuildTarget(
                    BuildTargetGroup.Android,
                    BuildTarget.Android);

                if (!switched)
                {
                    Debug.LogError("[Android Build] Could not switch to Android.");
                    return false;
                }
            }

            PlayerSettings.productName = profile.ProductName;
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.Android,
                profile.ApplicationIdentifier);
            PlayerSettings.bundleVersion = profile.BundleVersion;
            PlayerSettings.Android.bundleVersionCode = profile.BundleVersionCode;
            PlayerSettings.SetScriptingBackend(
                NamedBuildTarget.Android,
                ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetManagedStrippingLevel(
                NamedBuildTarget.Android,
                ManagedStrippingLevel.Medium);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.defaultInterfaceOrientation =
                UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            EditorUserBuildSettings.buildAppBundle = profile.BuildAppBundle;
            EditorUserBuildSettings.development = profile.DevelopmentBuild;
            EditorUserBuildSettings.allowDebugging = profile.AllowDebugging;

            AssignBootstrapEdition(profile.EditionConfig);
            EditorBuildSettings.scenes = GetRequiredScenes()
                .Select(path => new EditorBuildSettingsScene(path, true))
                .ToArray();

            AssetDatabase.SaveAssets();

            string message =
                $"[Android Build] Applied {profile.Flavor}: " +
                $"{profile.ApplicationIdentifier}, v{profile.BundleVersion} " +
                $"({profile.BundleVersionCode}), " +
                $"IL2CPP/ARM64, AAB={profile.BuildAppBundle}.";
            Debug.Log(message, profile);

            if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    "Android Build Profile",
                    $"Perfil aplicado: {profile.Flavor}\n\n" +
                    $"Producto: {profile.ProductName}\n" +
                    $"ID: {profile.ApplicationIdentifier}\n" +
                    $"Versión: {profile.BundleVersion} ({profile.BundleVersionCode})",
                    "OK");
            }

            return true;
        }

        public static void Build(AndroidBuildProfile profile)
        {
            if (!Apply(profile, false))
            {
                return;
            }

            List<string> problems = ValidateAppliedSettings(profile);
            if (problems.Count > 0)
            {
                ReportProblems(profile, problems, true);
                return;
            }

            string outputFolder = string.IsNullOrWhiteSpace(profile.OutputFolder)
                ? "Builds/Android"
                : profile.OutputFolder;
            Directory.CreateDirectory(outputFolder);

            string edition = profile.Flavor.ToString().Contains("Premium")
                ? "Premium"
                : "Free";
            string configuration = profile.DevelopmentBuild
                ? "Development"
                : "Release";
            string extension = profile.BuildAppBundle ? ".aab" : ".apk";
            string fileName =
                $"UltimatePuzzlesDinos_{edition}_{configuration}_" +
                $"v{Sanitize(profile.BundleVersion)}_{profile.BundleVersionCode}{extension}";
            string outputPath = Path.Combine(outputFolder, fileName)
                .Replace('\\', '/');

            BuildOptions options = BuildOptions.None;
            if (profile.DevelopmentBuild)
            {
                options |= BuildOptions.Development;
            }
            if (profile.AllowDebugging)
            {
                options |= BuildOptions.AllowDebugging;
            }

            BuildPlayerOptions buildOptions = new()
            {
                scenes = GetRequiredScenes(),
                locationPathName = outputPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = options
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log(
                    $"[Android Build] Build completed: {outputPath} " +
                    $"({summary.totalSize} bytes).",
                    profile);
                EditorUtility.RevealInFinder(outputPath);
                return;
            }

            Debug.LogError(
                $"[Android Build] Build failed: {summary.result}. " +
                $"Errors: {summary.totalErrors}, warnings: {summary.totalWarnings}.",
                profile);
        }

        public static List<string> ValidateProfile(AndroidBuildProfile profile)
        {
            List<string> problems = new();
            if (profile == null)
            {
                problems.Add("El perfil no existe.");
                return problems;
            }

            if (profile.EditionConfig == null)
                problems.Add("EditionConfig no está asignado.");
            if (string.IsNullOrWhiteSpace(profile.ProductName))
                problems.Add("Product Name está vacío.");
            if (string.IsNullOrWhiteSpace(profile.ApplicationIdentifier) ||
                profile.ApplicationIdentifier.Count(c => c == '.') < 2)
                problems.Add("Application Identifier no es válido.");
            if (string.IsNullOrWhiteSpace(profile.BundleVersion))
                problems.Add("Bundle Version está vacío.");
            if (profile.BundleVersionCode < 1)
                problems.Add("Bundle Version Code debe ser mayor que cero.");

            foreach (string scene in GetRequiredScenes())
            {
                if (!File.Exists(scene))
                    problems.Add($"Falta la escena: {scene}");
            }

            return problems;
        }

        public static List<string> ValidateAppliedSettings(AndroidBuildProfile profile)
        {
            List<string> problems = ValidateProfile(profile);
            if (PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android) != profile.ApplicationIdentifier)
                problems.Add("El Application Identifier aplicado no coincide con el perfil.");
            if (PlayerSettings.bundleVersion != profile.BundleVersion)
                problems.Add("Bundle Version aplicado no coincide con el perfil.");
            if (PlayerSettings.Android.bundleVersionCode != profile.BundleVersionCode)
                problems.Add("Bundle Version Code aplicado no coincide con el perfil.");
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
                problems.Add("Android no está usando IL2CPP.");
            if ((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) == 0)
                problems.Add("ARM64 no está habilitado.");
            if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.AutoRotation)
                problems.Add("La orientación debe estar en Auto Rotation.");
            if (PlayerSettings.allowedAutorotateToPortrait ||
                PlayerSettings.allowedAutorotateToPortraitUpsideDown)
                problems.Add("Portrait debe estar deshabilitado.");
            if (!PlayerSettings.allowedAutorotateToLandscapeLeft ||
                !PlayerSettings.allowedAutorotateToLandscapeRight)
                problems.Add("Landscape Left y Right deben estar habilitados.");
            return problems;
        }

        private static void AssignBootstrapEdition(EditionConfig editionConfig)
        {
            string configPath = AssetDatabase.GetAssetPath(editionConfig);
            Scene scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            EditionConfig reloadedConfig =
                AssetDatabase.LoadAssetAtPath<EditionConfig>(configPath);

            AppBootstrap bootstrap = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<AppBootstrap>(true))
                .FirstOrDefault();

            if (bootstrap == null || reloadedConfig == null)
                throw new InvalidOperationException(
                    "Could not assign the edition to Bootstrap.unity.");

            SerializedObject serialized = new(bootstrap);
            SerializedProperty property = serialized.FindProperty("editionConfig");
            if (property == null)
                throw new InvalidOperationException(
                    "AppBootstrap.editionConfig was not found.");

            property.objectReferenceValue = reloadedConfig;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static string[] GetRequiredScenes() => new[]
        {
            "Assets/_Project/Scenes/Bootstrap.unity",
            "Assets/_Project/Scenes/MainMenu.unity",
            "Assets/_Project/Scenes/LevelSelection.unity",
            "Assets/_Project/Scenes/DifficultySelection.unity",
            "Assets/_Project/Scenes/Gameplay.unity"
        };

        private static string Sanitize(string value)
        {
            foreach (char invalid in Path.GetInvalidFileNameChars())
                value = value.Replace(invalid, '_');
            return value.Replace('.', '_');
        }

        private static void ReportProblems(
            AndroidBuildProfile profile,
            IReadOnlyList<string> problems,
            bool showDialog)
        {
            string report = string.Join("\n- ", problems);
            Debug.LogError(
                $"[Android Build] Profile validation failed " +
                $"({profile?.name ?? "null"}):\n- {report}",
                profile);

            if (showDialog)
            {
                EditorUtility.DisplayDialog(
                    "Android Build Validation",
                    "Problemas encontrados:\n\n- " + report,
                    "OK");
            }
        }
    }
}
#endif
