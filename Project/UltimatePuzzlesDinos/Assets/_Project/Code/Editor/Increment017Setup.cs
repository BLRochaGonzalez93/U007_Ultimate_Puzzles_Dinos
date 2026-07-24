using System.IO;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Audio;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment017Setup
    {
        private const string AudioResourcesFolder = "Assets/_Project/Resources/Audio";
        private const string AudioConfigPath = AudioResourcesFolder + "/AudioConfig.asset";
        public static void Run()
        {
            EnsureFolder("Assets/_Project", "Resources");
            EnsureFolder("Assets/_Project/Resources", "Audio");
            EnsureFolder("Assets/_Project/Art/Audio", "Music");
            EnsureFolder("Assets/_Project/Art/Audio", "SFX");

            AudioConfig config = AssetDatabase.LoadAssetAtPath<AudioConfig>(AudioConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AudioConfig>();
                AssetDatabase.CreateAsset(config, AudioConfigPath);
            }

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = config;

            Debug.Log(
                "[Increment 017] AudioConfig preparado. " +
                "Asigna los clips desde el Inspector; el juego seguirá funcionando si quedan vacíos.",
                config);
        }
        public static void Validate()
        {
            bool valid = true;

            AudioConfig config = AssetDatabase.LoadAssetAtPath<AudioConfig>(AudioConfigPath);
            if (config == null)
            {
                valid = false;
                Debug.LogError($"[Increment 017] Falta {AudioConfigPath}.");
            }

            string[] requiredScripts =
            {
                "Assets/_Project/Code/Runtime/Audio/AudioCue.cs",
                "Assets/_Project/Code/Runtime/Audio/AudioConfig.cs",
                "Assets/_Project/Code/Runtime/Audio/AudioService.cs",
                "Assets/_Project/Code/Runtime/Audio/UIButtonAudioFeedback.cs"
            };

            foreach (string script in requiredScripts)
            {
                if (!File.Exists(script))
                {
                    valid = false;
                    Debug.LogError($"[Increment 017] Falta {script}.");
                }
            }

            if (valid)
            {
                Debug.Log("[Increment 017] Validación correcta.");
            }
        }

        private static void EnsureFolder(string parent, string child)
        {
            string fullPath = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
