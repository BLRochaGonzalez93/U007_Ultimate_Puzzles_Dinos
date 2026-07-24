#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Performance;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment028PerformanceSetup
    {
        private const string ConfigFolder =
            "Assets/_Project/Resources/Performance";

        private const string ConfigPath =
            ConfigFolder + "/MobilePerformanceConfig.asset";

        private const string PuzzleFolder =
            "Assets/_Project/Art/Sprites/Puzzles";

        private const string UiFolder =
            "Assets/_Project/Art/Sprites/UI";

        private const string MusicFolder =
            "Assets/_Project/Art/Audio/Music";

        private const string SfxFolder =
            "Assets/_Project/Art/Audio/SFX";

        public static void Run()
        {
            EnsureFolder("Assets/_Project/Resources");
            EnsureFolder(ConfigFolder);

            MobilePerformanceConfig config =
                AssetDatabase.LoadAssetAtPath
                    <MobilePerformanceConfig>(ConfigPath);

            if (config == null)
            {
                config =
                    ScriptableObject.CreateInstance
                        <MobilePerformanceConfig>();

                AssetDatabase.CreateAsset(config, ConfigPath);
            }

            ApplyAndroidTextureSettings(
                PuzzleFolder,
                maxTextureSize: 1024,
                TextureImporterFormat.ASTC_6x6);

            ApplyAndroidTextureSettings(
                UiFolder,
                maxTextureSize: 2048,
                TextureImporterFormat.ASTC_4x4);

            ApplyMusicImportSettings();
            ApplySfxImportSettings();

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = config;

            Debug.Log(
                "[Increment 028] Android performance settings applied.");

            EditorUtility.DisplayDialog(
                "Ultimate Puzzles Dinos",
                "Increment 028 completado.\n\n" +
                "Se han optimizado texturas, música, SFX y " +
                "configuración de rendimiento móvil.",
                "OK");
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Performance/Use Low Tier")]
        public static void UseLowTier()
        {
            SetTier(MobilePerformanceTier.Low);
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Performance/Use Medium Tier")]
        public static void UseMediumTier()
        {
            SetTier(MobilePerformanceTier.Medium);
        }

        [MenuItem(
            "VRM Games/Ultimate Puzzles Dinos/Performance/Use High Tier")]
        public static void UseHighTier()
        {
            SetTier(MobilePerformanceTier.High);
        }

        public static void Validate()
        {
            List<string> problems = new();

            MobilePerformanceConfig config =
                AssetDatabase.LoadAssetAtPath
                    <MobilePerformanceConfig>(ConfigPath);

            if (config == null)
            {
                problems.Add(
                    $"Falta el asset {ConfigPath}.");
            }

            ValidateTextureFolder(
                PuzzleFolder,
                1024,
                TextureImporterFormat.ASTC_6x6,
                problems);

            ValidateTextureFolder(
                UiFolder,
                2048,
                TextureImporterFormat.ASTC_4x4,
                problems);

            ValidateAudioFolder(
                MusicFolder,
                AudioClipLoadType.Streaming,
                problems);

            ValidateAudioFolder(
                SfxFolder,
                AudioClipLoadType.DecompressOnLoad,
                problems);

            if (problems.Count == 0)
            {
                Debug.Log(
                    "[Increment 028] Validación correcta.");

                EditorUtility.DisplayDialog(
                    "Validación correcta",
                    "Increment 028 instalado correctamente.",
                    "OK");

                return;
            }

            string report =
                string.Join("\n- ", problems);

            Debug.LogError(
                "[Increment 028] Validación fallida:\n- " +
                report);

            EditorUtility.DisplayDialog(
                "Validación fallida",
                "Problemas encontrados:\n\n- " +
                report,
                "OK");
        }

        private static void SetTier(
            MobilePerformanceTier tier)
        {
            MobilePerformanceConfig config =
                AssetDatabase.LoadAssetAtPath
                    <MobilePerformanceConfig>(ConfigPath);

            if (config == null)
            {
                Debug.LogError(
                    "[Performance] Ejecuta primero " +
                    "la configuración de rendimiento.");

                return;
            }

            config.SetActiveTier(tier);
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            Selection.activeObject = config;

            Debug.Log(
                $"[Performance] Active tier set to {tier}.");
        }

        private static void ApplyAndroidTextureSettings(
            string folder,
            int maxTextureSize,
            TextureImporterFormat format)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string[] textureGuids =
                AssetDatabase.FindAssets(
                    "t:Texture2D",
                    new[] { folder });

            foreach (string guid in textureGuids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                TextureImporter importer =
                    AssetImporter.GetAtPath(path)
                        as TextureImporter;

                if (importer == null)
                {
                    continue;
                }

                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.textureCompression =
                    TextureImporterCompression.Compressed;

                TextureImporterPlatformSettings android =
                    importer.GetPlatformTextureSettings("Android");

                android.name = "Android";
                android.overridden = true;
                android.maxTextureSize = maxTextureSize;
                android.format = format;
                android.compressionQuality = 60;
                android.crunchedCompression = false;

                importer.SetPlatformTextureSettings(android);
                importer.SaveAndReimport();
            }
        }

        private static void ApplyMusicImportSettings()
        {
            ApplyAudioImportSettings(
                MusicFolder,
                AudioClipLoadType.Streaming,
                AudioCompressionFormat.Vorbis,
                0.65f,
                preloadAudioData: false);
        }

        private static void ApplySfxImportSettings()
        {
            ApplyAudioImportSettings(
                SfxFolder,
                AudioClipLoadType.DecompressOnLoad,
                AudioCompressionFormat.Vorbis,
                0.75f,
                preloadAudioData: true);
        }

        private static void ApplyAudioImportSettings(
            string folder,
            AudioClipLoadType loadType,
            AudioCompressionFormat compressionFormat,
            float quality,
            bool preloadAudioData)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string[] audioGuids =
                AssetDatabase.FindAssets(
                    "t:AudioClip",
                    new[] { folder });

            foreach (string guid in audioGuids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                AudioImporter importer =
                    AssetImporter.GetAtPath(path)
                        as AudioImporter;

                if (importer == null)
                {
                    continue;
                }

                AudioImporterSampleSettings settings =
    importer.defaultSampleSettings;

                settings.loadType = loadType;
                settings.compressionFormat =
                    compressionFormat;
                settings.quality = quality;
                settings.sampleRateSetting =
                    AudioSampleRateSetting.OptimizeSampleRate;
                settings.preloadAudioData = preloadAudioData;

                importer.defaultSampleSettings = settings;
                importer.loadInBackground =
                    loadType == AudioClipLoadType.Streaming;
                importer.forceToMono = false;

                importer.SaveAndReimport();
            }
        }

        private static void ValidateTextureFolder(
            string folder,
            int maxTextureSize,
            TextureImporterFormat expectedFormat,
            List<string> problems)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                problems.Add($"Falta la carpeta {folder}.");
                return;
            }

            string[] guids =
                AssetDatabase.FindAssets(
                    "t:Texture2D",
                    new[] { folder });

            foreach (string guid in guids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                TextureImporter importer =
                    AssetImporter.GetAtPath(path)
                        as TextureImporter;

                if (importer == null)
                {
                    continue;
                }

                TextureImporterPlatformSettings android =
                    importer.GetPlatformTextureSettings("Android");

                if (!android.overridden ||
                    android.maxTextureSize != maxTextureSize ||
                    android.format != expectedFormat)
                {
                    problems.Add(
                        $"Configuración Android incorrecta: {path}");
                }
            }
        }

        private static void ValidateAudioFolder(
            string folder,
            AudioClipLoadType expectedLoadType,
            List<string> problems)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                problems.Add($"Falta la carpeta {folder}.");
                return;
            }

            string[] guids =
                AssetDatabase.FindAssets(
                    "t:AudioClip",
                    new[] { folder });

            foreach (string guid in guids)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guid);

                AudioImporter importer =
                    AssetImporter.GetAtPath(path)
                        as AudioImporter;

                if (importer == null)
                {
                    continue;
                }

                if (importer.defaultSampleSettings.loadType !=
                    expectedLoadType)
                {
                    problems.Add(
                        $"Load Type incorrecto: {path}");
                }
            }
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string normalized =
                folderPath.Replace('\\', '/');

            string parent =
                Path.GetDirectoryName(normalized)
                    ?.Replace('\\', '/');

            string folderName =
                Path.GetFileName(normalized);

            if (!string.IsNullOrEmpty(parent) &&
                !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
#endif
