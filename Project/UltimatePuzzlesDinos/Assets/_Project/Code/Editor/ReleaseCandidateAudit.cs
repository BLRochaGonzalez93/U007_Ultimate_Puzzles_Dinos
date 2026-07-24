#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Application;
using VRMGames.UltimatePuzzlesDinos.Configuration;
using VRMGames.UltimatePuzzlesDinos.Content;
using VRMGames.UltimatePuzzlesDinos.Performance;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class ReleaseCandidateAudit
    {
        private const string MenuRoot =
            "VRM Games/Ultimate Puzzles Dinos/Release/";

        private static readonly string[] RequiredScenes =
        {
            "Assets/_Project/Scenes/Bootstrap.unity",
            "Assets/_Project/Scenes/MainMenu.unity",
            "Assets/_Project/Scenes/LevelSelection.unity",
            "Assets/_Project/Scenes/DifficultySelection.unity",
            "Assets/_Project/Scenes/Gameplay.unity"
        };

        [MenuItem(MenuRoot + "Run Release Candidate Audit")]
        public static void RunAudit()
        {
            AuditReport report = new();
            ValidateScenes(report);
            ValidateBootstrap(report);
            ValidateContent(report);
            ValidateLevelCards(report);
            ValidateAudio(report);
            ValidatePerformance(report);
            ValidateAndroid(report);
            ValidatePublishing(report);
            ValidateEditorMenus(report);

            string text = report.Build();
            const string reportFolder = "Assets/_Project/Documentation/QA";
            const string reportPath = reportFolder + "/ReleaseCandidateAudit.txt";
            EnsureFolder("Assets/_Project/Documentation");
            EnsureFolder(reportFolder);
            File.WriteAllText(reportPath, text);
            AssetDatabase.ImportAsset(reportPath);

            if (report.Errors.Count > 0)
            {
                Debug.LogError("[Release Candidate Audit]\n" + text);
                EditorUtility.DisplayDialog(
                    "Release Candidate: NO LISTA",
                    $"Errores: {report.Errors.Count}\n" +
                    $"Avisos: {report.Warnings.Count}\n\n" +
                    "Revisa ReleaseCandidateAudit.txt.",
                    "OK");
                return;
            }

            if (report.Warnings.Count > 0)
            {
                Debug.LogWarning("[Release Candidate Audit]\n" + text);
                EditorUtility.DisplayDialog(
                    "Release Candidate: LISTA CON AVISOS",
                    $"Sin errores bloqueantes.\nAvisos: {report.Warnings.Count}\n\n" +
                    "Revisa ReleaseCandidateAudit.txt.",
                    "OK");
                return;
            }

            Debug.Log("[Release Candidate Audit]\n" + text);
            EditorUtility.DisplayDialog(
                "Release Candidate: LISTA",
                "No se han detectado errores ni avisos.",
                "OK");
        }

        private static void ValidateScenes(AuditReport report)
        {
            foreach (string path in RequiredScenes)
                if (!File.Exists(path)) report.Error("Falta escena: " + path);

            string[] enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
            foreach (string path in RequiredScenes)
                if (!enabledScenes.Contains(path))
                    report.Error("Escena no incluida/activa en Build Settings: " + path);
        }

        private static void ValidateBootstrap(AuditReport report)
        {
            string path = RequiredScenes[0];
            if (!File.Exists(path)) return;

            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            try
            {
                AppBootstrap bootstrap = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<AppBootstrap>(true))
                    .FirstOrDefault();
                if (bootstrap == null)
                {
                    report.Error("Bootstrap.unity no contiene AppBootstrap.");
                }
                else
                {
                    SerializedObject serialized = new(bootstrap);
                    SerializedProperty edition = serialized.FindProperty("editionConfig");
                    if (edition == null || edition.objectReferenceValue == null)
                        report.Error("AppBootstrap no tiene EditionConfig asignado.");
                }
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }

            ValidateEditionAsset("Assets/_Project/Config/Editions/Edition_Free.asset", false, report);
            ValidateEditionAsset("Assets/_Project/Config/Editions/Edition_Premium.asset", true, report);
        }

        private static void ValidateEditionAsset(string path, bool premium, AuditReport report)
        {
            EditionConfig config = AssetDatabase.LoadAssetAtPath<EditionConfig>(path);
            if (config == null)
            {
                report.Error("Falta EditionConfig: " + path);
                return;
            }
            if (premium && !config.AllContentUnlocked)
                report.Warning("Premium debería tener All Content Unlocked activado.");
            if (premium && config.AdsEnabled)
                report.Error("Premium no debe tener Ads Enabled activado.");
            if (!premium && !config.RewardedUnlocksEnabled)
                report.Warning("Free no tiene Rewarded Unlocks activado.");
        }

        private static void ValidateContent(AuditReport report)
        {
            string[] definitionGuids = AssetDatabase.FindAssets(
                "t:PuzzleDefinition", new[] { "Assets/_Project" });
            List<PuzzleDefinition> definitions = definitionGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PuzzleDefinition>)
                .Where(item => item != null)
                .ToList();

            if (definitions.Count != 60)
                report.Error($"Se esperaban 60 PuzzleDefinition y hay {definitions.Count}.");

            HashSet<string> ids = new(StringComparer.OrdinalIgnoreCase);
            foreach (PuzzleDefinition definition in definitions)
            {
                if (definition.Image == null)
                    report.Error($"{definition.name} no tiene imagen asignada.");
                if (string.IsNullOrWhiteSpace(definition.Id))
                    report.Error($"{definition.name} tiene Id vacío.");
                else if (!ids.Add(definition.Id))
                    report.Error($"Puzzle Id duplicado: {definition.Id}.");
            }
        }

        private static void ValidateLevelCards(AuditReport report)
        {
            const string path = "Assets/_Project/Scenes/LevelSelection.unity";
            if (!File.Exists(path)) return;
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            try
            {
                GameObject[] roots = scene.GetRootGameObjects();
                for (int level = 1; level <= 60; level++)
                {
                    Transform card = Find(roots, $"LevelCard_{level:00}");
                    if (card == null)
                    {
                        report.Error($"Falta LevelCard_{level:00}.");
                        continue;
                    }
                    Button button = card.GetComponent<Button>();
                    Image image = card.GetComponent<Image>();
                    if (button == null) report.Error($"LevelCard_{level:00} no tiene Button.");
                    if (image == null) report.Error($"LevelCard_{level:00} no tiene Image.");
                    if (button != null && button.targetGraphic == null)
                        report.Error($"LevelCard_{level:00} no tiene Target Graphic.");
                    if (Find(card, "Preview") == null)
                        report.Error($"LevelCard_{level:00} no tiene Preview.");
                    Transform lockCover = Find(card, "LockCover");
                    if (lockCover == null)
                        report.Error($"LevelCard_{level:00} no tiene LockCover.");
                    else
                    {
                        Graphic graphic = lockCover.GetComponent<Graphic>();
                        if (graphic != null && graphic.raycastTarget)
                            report.Error($"LevelCard_{level:00}/LockCover bloquea raycasts.");
                    }
                    if (Find(card, "LockIcon") != null)
                        report.Warning($"LevelCard_{level:00} conserva un LockIcon antiguo.");
                }
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static void ValidateAudio(AuditReport report)
        {
            const string path = "Assets/_Project/Resources/Audio/AudioConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path) == null)
                report.Error("Falta AudioConfig.asset.");
            if (AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/_Project/Art/Audio/Music" }).Length == 0)
                report.Warning("No hay música importada.");
            if (AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/_Project/Art/Audio/SFX" }).Length == 0)
                report.Warning("No hay SFX importados.");
        }

        private static void ValidatePerformance(AuditReport report)
        {
            const string path = "Assets/_Project/Resources/Performance/MobilePerformanceConfig.asset";
            if (AssetDatabase.LoadAssetAtPath<MobilePerformanceConfig>(path) == null)
                report.Error("Falta MobilePerformanceConfig.asset.");
        }

        private static void ValidateAndroid(AuditReport report)
        {
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) != ScriptingImplementation.IL2CPP)
                report.Error("Android no usa IL2CPP.");
            if ((PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) == 0)
                report.Error("ARM64 no está habilitado.");
            if (!EditorUserBuildSettings.buildAppBundle)
                report.Warning("Build App Bundle no está activado actualmente.");
            string identifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
            if (string.IsNullOrWhiteSpace(identifier))
                report.Error("Application Identifier Android está vacío.");
        }

        private static void ValidatePublishing(AuditReport report)
        {
            const string configPath = "Assets/_Project/Config/Publishing/AndroidPublishingConfig.asset";
            AndroidPublishingConfig config = AssetDatabase.LoadAssetAtPath<AndroidPublishingConfig>(configPath);
            if (config == null)
            {
                report.Error("Falta AndroidPublishingConfig.asset.");
                return;
            }
            if (config.ApplicationIcon == null) report.Error("Application Icon no está asignado.");
            if (string.IsNullOrWhiteSpace(config.PrivacyPolicyUrl)) report.Warning("Privacy Policy URL pendiente.");
            if (string.IsNullOrWhiteSpace(config.SupportEmail)) report.Warning("Support Email pendiente.");
            if (string.IsNullOrWhiteSpace(config.WebsiteUrl)) report.Warning("Website URL pendiente.");

            string[] profiles = AssetDatabase.FindAssets(
                "t:AndroidBuildProfile", new[] { "Assets/_Project/Config/BuildProfiles" });
            if (profiles.Length != 4)
                report.Error($"Se esperaban 4 AndroidBuildProfile y hay {profiles.Length}.");
        }

        private static void ValidateEditorMenus(AuditReport report)
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:MonoScript", new[] { "Assets/_Project/Code/Editor" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (Path.GetFileName(path) == "ReleaseCandidateAudit.cs")
                {
                    continue;
                }

                string source = File.ReadAllText(path);
                int searchIndex = 0;

                while (true)
                {
                    int menuStart = source.IndexOf(
                        "[MenuItem",
                        searchIndex,
                        StringComparison.Ordinal);

                    if (menuStart < 0)
                    {
                        break;
                    }

                    int menuEnd = source.IndexOf(
                        ")]",
                        menuStart,
                        StringComparison.Ordinal);

                    if (menuEnd < 0)
                    {
                        break;
                    }

                    string menuBlock = source.Substring(
                        menuStart,
                        menuEnd - menuStart + 2);

                    if (menuBlock.IndexOf(
                            "/Run Increment ",
                            StringComparison.Ordinal) >= 0)
                    {
                        report.Error(
                            "Queda un Run Increment en el menú: " +
                            Path.GetFileName(path));
                    }

                    if (menuBlock.IndexOf(
                            "/Validate Increment ",
                            StringComparison.Ordinal) >= 0)
                    {
                        report.Error(
                            "Queda un Validate Increment en el menú: " +
                            Path.GetFileName(path));
                    }

                    if (menuBlock.IndexOf(
                            "/Responsive Preview/",
                            StringComparison.Ordinal) >= 0)
                    {
                        report.Warning(
                            "Queda una herramienta Responsive Preview " +
                            "obsoleta: " + Path.GetFileName(path));
                    }

                    searchIndex = menuEnd + 2;
                }
            }
        }

        private static Transform Find(GameObject[] roots, string name)
        {
            foreach (GameObject root in roots)
            {
                Transform result = Find(root.transform, name);
                if (result != null) return result;
            }
            return null;
        }

        private static Transform Find(Transform root, string name)
        {
            if (root == null) return null;
            if (root.name == name) return root;
            foreach (Transform child in root)
            {
                Transform result = Find(child, name);
                if (result != null) return result;
            }
            return null;
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            string normalized = folder.Replace('\\', '/');
            string parent = Path.GetDirectoryName(normalized)?.Replace('\\', '/');
            string name = Path.GetFileName(normalized);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);
            if (!string.IsNullOrEmpty(parent)) AssetDatabase.CreateFolder(parent, name);
        }

        private sealed class AuditReport
        {
            public List<string> Errors { get; } = new();
            public List<string> Warnings { get; } = new();
            public void Error(string message) => Errors.Add(message);
            public void Warning(string message) => Warnings.Add(message);
            public string Build()
            {
                List<string> output = new()
                {
                    "ULTIMATE PUZZLES DINOS - RELEASE CANDIDATE AUDIT",
                    "Generated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "",
                    $"Errors: {Errors.Count}",
                    $"Warnings: {Warnings.Count}",
                    ""
                };
                if (Errors.Count > 0)
                {
                    output.Add("BLOCKING ERRORS");
                    output.AddRange(Errors.Select(item => "- " + item));
                    output.Add("");
                }
                if (Warnings.Count > 0)
                {
                    output.Add("WARNINGS");
                    output.AddRange(Warnings.Select(item => "- " + item));
                    output.Add("");
                }
                if (Errors.Count == 0 && Warnings.Count == 0)
                    output.Add("No issues detected.");
                return string.Join("\n", output);
            }
        }
    }
}
#endif
