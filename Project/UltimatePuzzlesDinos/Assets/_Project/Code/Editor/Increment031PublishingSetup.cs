#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment031PublishingSetup
    {
        private const string ConfigFolder =
            "Assets/_Project/Config/Publishing";

        private const string ConfigPath =
            ConfigFolder + "/AndroidPublishingConfig.asset";

        private const string IconPath =
            "Assets/_Project/Art/Sprites/UI/Logo512.png";

        private const string ProfilesFolder =
            "Assets/_Project/Config/BuildProfiles";

        public static void Run()
        {
            EnsureFolder("Assets/_Project/Config");
            EnsureFolder(ConfigFolder);

            AndroidPublishingConfig config =
                AssetDatabase.LoadAssetAtPath
                    <AndroidPublishingConfig>(ConfigPath);

            if (config == null)
            {
                config =
                    ScriptableObject.CreateInstance
                        <AndroidPublishingConfig>();

                AssetDatabase.CreateAsset(config, ConfigPath);
            }

            Texture2D icon =
                AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);

            SerializedObject serialized = new(config);
            SerializedProperty iconProperty =
                serialized.FindProperty("applicationIcon");

            if (iconProperty != null && iconProperty.objectReferenceValue == null)
            {
                iconProperty.objectReferenceValue = icon;
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);

            ConfigureIconImporter();
            ApplyBranding(config);
            UpdateBuildProfiles(config);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;

            Debug.Log(
                "[Increment 031] Android publishing branding applied.");

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 031 completado.\n\n" +
                "Branding, versionado y metadatos de publicación preparados.",
                "OK");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Publishing/Apply Branding")]
        public static void ApplyBrandingFromMenu()
        {
            AndroidPublishingConfig config =
                AssetDatabase.LoadAssetAtPath
                    <AndroidPublishingConfig>(ConfigPath);

            if (config == null)
            {
                Debug.LogError(
                    "[Publishing] Falta AndroidPublishingConfig.asset.");
                return;
            }

            ApplyBranding(config);
            UpdateBuildProfiles(config);
            AssetDatabase.SaveAssets();

            Debug.Log("[Publishing] Branding reapplied.");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Publishing/Open Publishing Config")]
        public static void OpenPublishingConfig()
        {
            AndroidPublishingConfig config =
                AssetDatabase.LoadAssetAtPath
                    <AndroidPublishingConfig>(ConfigPath);

            if (config == null)
            {
                Debug.LogError(
                    "[Publishing] Falta AndroidPublishingConfig.asset.");
                return;
            }

            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);
        }

        public static void Validate()
        {
            List<string> errors = new();
            List<string> warnings = new();

            AndroidPublishingConfig config =
                AssetDatabase.LoadAssetAtPath
                    <AndroidPublishingConfig>(ConfigPath);

            if (config == null)
            {
                errors.Add($"Falta {ConfigPath}.");
            }
            else
            {
                ValidateConfig(config, errors, warnings);
            }

            ValidateProfiles(errors, warnings);

            if (!File.Exists(IconPath))
            {
                errors.Add($"Falta el icono fuente {IconPath}.");
            }

            string message = BuildReport(errors, warnings);

            if (errors.Count > 0)
            {
                Debug.LogError(
                    "[Increment 031] Validación fallida.\n" + message);

                EditorUtility.DisplayDialog(
                    "Validación fallida",
                    message,
                    "OK");

                return;
            }

            if (warnings.Count > 0)
            {
                Debug.LogWarning(
                    "[Increment 031] Validación completada con avisos.\n" +
                    message);

                EditorUtility.DisplayDialog(
                    "Validación con avisos",
                    message,
                    "OK");

                return;
            }

            Debug.Log("[Increment 031] Validación correcta.");

            EditorUtility.DisplayDialog(
                "Validación correcta",
                "Branding y configuración de publicación correctos.",
                "OK");
        }

        private static void ApplyBranding(
            AndroidPublishingConfig config)
        {
            if (config == null)
            {
                return;
            }

            PlayerSettings.companyName = config.CompanyName;
            PlayerSettings.bundleVersion = config.BundleVersion;

            PlayerSettings.SplashScreen.showUnityLogo =
                config.ShowUnityLogo;

            PlayerSettings.SplashScreen.backgroundColor =
                config.SplashBackground;

            if (config.ApplicationIcon != null)
            {
                TryAssignAndroidIcon(config.ApplicationIcon);
            }
        }

        private static void UpdateBuildProfiles(
            AndroidPublishingConfig config)
        {
            if (!AssetDatabase.IsValidFolder(ProfilesFolder))
            {
                Debug.LogWarning(
                    "[Publishing] No existe la carpeta de BuildProfiles. " +
                    "Se omite su actualización.");

                return;
            }

            string[] guids =
                AssetDatabase.FindAssets(
                    "t:AndroidBuildProfile",
                    new[] { ProfilesFolder });

            foreach (string guid in guids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                AndroidBuildProfile profile =
                    AssetDatabase.LoadAssetAtPath
                        <AndroidBuildProfile>(path);

                if (profile == null)
                {
                    continue;
                }

                bool premium =
                    profile.Flavor == AndroidBuildFlavor.PremiumDevelopment ||
                    profile.Flavor == AndroidBuildFlavor.PremiumRelease;

                SerializedObject serialized = new(profile);

                SerializedProperty productName =
                    serialized.FindProperty("productName");

                SerializedProperty bundleVersion =
                    serialized.FindProperty("bundleVersion");

                SerializedProperty bundleVersionCode =
                    serialized.FindProperty("bundleVersionCode");

                if (productName != null)
                {
                    string baseName = premium
                        ? config.PremiumProductName
                        : config.FreeProductName;

                    bool development =
                        profile.Flavor == AndroidBuildFlavor.FreeDevelopment ||
                        profile.Flavor == AndroidBuildFlavor.PremiumDevelopment;

                    productName.stringValue =
                        development
                            ? baseName + " DEV"
                            : baseName;
                }

                if (bundleVersion != null)
                {
                    bundleVersion.stringValue =
                        config.BundleVersion;
                }

                if (bundleVersionCode != null)
                {
                    bundleVersionCode.intValue =
                        premium
                            ? config.PremiumBundleVersionCode
                            : config.FreeBundleVersionCode;
                }

                serialized.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(profile);
            }
        }

        private static void ConfigureIconImporter()
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(IconPath)
                    as TextureImporter;

            if (importer == null)
            {
                return;
            }

            bool changed = false;

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (importer.alphaIsTransparency == false)
            {
                importer.alphaIsTransparency = true;
                changed = true;
            }

            TextureImporterPlatformSettings android =
                importer.GetPlatformTextureSettings("Android");

            if (!android.overridden ||
                android.maxTextureSize < 512)
            {
                android.name = "Android";
                android.overridden = true;
                android.maxTextureSize = 512;
                android.format = TextureImporterFormat.ASTC_4x4;
                android.compressionQuality = 80;
                importer.SetPlatformTextureSettings(android);
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }

        private static void TryAssignAndroidIcon(Texture2D icon)
        {
            try
            {
                Type playerSettingsType = typeof(PlayerSettings);

                MethodInfo legacyMethod =
                    playerSettingsType.GetMethod(
                        "SetIconsForTargetGroup",
                        BindingFlags.Public |
                        BindingFlags.Static,
                        null,
                        new[]
                        {
                            typeof(BuildTargetGroup),
                            typeof(Texture2D[])
                        },
                        null);

                if (legacyMethod != null)
                {
                    legacyMethod.Invoke(
                        null,
                        new object[]
                        {
                            BuildTargetGroup.Android,
                            new[] { icon }
                        });

                    Debug.Log(
                        "[Publishing] Android application icon assigned.");

                    return;
                }

                Debug.LogWarning(
                    "[Publishing] La API de iconos de esta versión de Unity " +
                    "no expone SetIconsForTargetGroup(Texture2D[]). " +
                    "Logo512.png está preparado, pero revisa manualmente " +
                    "Player Settings > Android > Icons.");
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[Publishing] No se pudo asignar el icono " +
                    "automáticamente. Revisa Player Settings > Android > " +
                    $"Icons. Detalle: {exception.GetBaseException().Message}");
            }
        }

        private static void ValidateConfig(
            AndroidPublishingConfig config,
            List<string> errors,
            List<string> warnings)
        {
            if (string.IsNullOrWhiteSpace(config.CompanyName))
            {
                errors.Add("Company Name está vacío.");
            }

            if (string.IsNullOrWhiteSpace(config.FreeProductName))
            {
                errors.Add("Free Product Name está vacío.");
            }

            if (string.IsNullOrWhiteSpace(config.PremiumProductName))
            {
                errors.Add("Premium Product Name está vacío.");
            }

            if (string.IsNullOrWhiteSpace(config.BundleVersion))
            {
                errors.Add("Bundle Version está vacío.");
            }

            if (config.FreeBundleVersionCode < 1)
            {
                errors.Add(
                    "Free Bundle Version Code debe ser mayor que 0.");
            }

            if (config.PremiumBundleVersionCode < 1)
            {
                errors.Add(
                    "Premium Bundle Version Code debe ser mayor que 0.");
            }

            if (config.ApplicationIcon == null)
            {
                errors.Add("Application Icon no está asignado.");
            }

            if (string.IsNullOrWhiteSpace(config.PrivacyPolicyUrl))
            {
                warnings.Add(
                    "Privacy Policy URL todavía está vacío.");
            }

            if (string.IsNullOrWhiteSpace(config.SupportEmail))
            {
                warnings.Add(
                    "Support Email todavía está vacío.");
            }

            if (string.IsNullOrWhiteSpace(config.WebsiteUrl))
            {
                warnings.Add(
                    "Website URL todavía está vacío.");
            }
        }

        private static void ValidateProfiles(
            List<string> errors,
            List<string> warnings)
        {
            if (!AssetDatabase.IsValidFolder(ProfilesFolder))
            {
                errors.Add(
                    "No existe la carpeta de perfiles Android.");

                return;
            }

            string[] guids =
                AssetDatabase.FindAssets(
                    "t:AndroidBuildProfile",
                    new[] { ProfilesFolder });

            if (guids.Length != 4)
            {
                errors.Add(
                    $"Se esperaban 4 perfiles Android y hay {guids.Length}.");
            }

            foreach (string guid in guids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                AndroidBuildProfile profile =
                    AssetDatabase.LoadAssetAtPath
                        <AndroidBuildProfile>(path);

                if (profile == null)
                {
                    continue;
                }

                List<string> profileProblems =
                    AndroidBuildProfileService.ValidateProfile(profile);

                foreach (string problem in profileProblems)
                {
                    errors.Add(
                        $"{profile.name}: {problem}");
                }

                bool release =
                    profile.Flavor == AndroidBuildFlavor.FreeRelease ||
                    profile.Flavor == AndroidBuildFlavor.PremiumRelease;

                if (release && profile.DevelopmentBuild)
                {
                    errors.Add(
                        $"{profile.name}: Release no debe ser Development.");
                }

                if (release && profile.AllowDebugging)
                {
                    errors.Add(
                        $"{profile.name}: Release no debe permitir debugging.");
                }

                if (!profile.BuildAppBundle)
                {
                    warnings.Add(
                        $"{profile.name}: no está configurado como AAB.");
                }
            }
        }

        private static string BuildReport(
            IReadOnlyList<string> errors,
            IReadOnlyList<string> warnings)
        {
            List<string> sections = new();

            if (errors.Count > 0)
            {
                sections.Add(
                    "ERRORES:\n- " +
                    string.Join("\n- ", errors));
            }

            if (warnings.Count > 0)
            {
                sections.Add(
                    "AVISOS:\n- " +
                    string.Join("\n- ", warnings));
            }

            if (sections.Count == 0)
            {
                return "Sin incidencias.";
            }

            return string.Join("\n\n", sections);
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string normalized =
                folder.Replace('\\', '/');

            string parent =
                Path.GetDirectoryName(normalized)
                    ?.Replace('\\', '/');

            string name =
                Path.GetFileName(normalized);

            if (!string.IsNullOrEmpty(parent) &&
                !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            if (!string.IsNullOrEmpty(parent))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }
    }
}
#endif
