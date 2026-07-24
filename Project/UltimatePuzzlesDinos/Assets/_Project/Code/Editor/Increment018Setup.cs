using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Audio;

namespace VRMGames.UltimatePuzzlesDinos.EditorTools
{
    public static class Increment018Setup
    {
        private const string AudioConfigPath =
            "Assets/_Project/Resources/Audio/AudioConfig.asset";

        private static readonly string[] AudioFolders =
        {
            "Assets/_Project/Art/Audio/Music",
            "Assets/_Project/Art/Audio/SFX"
        };
        public static void Run()
        {
            AudioConfig config = AssetDatabase.LoadAssetAtPath<AudioConfig>(AudioConfigPath);
            if (config == null)
            {
                Debug.LogError(
                    $"[Increment 018] No existe {AudioConfigPath}. " +
                    "Ejecuta primero Run Increment 017.");
                return;
            }

            List<AudioClipEntry> clips = FindProjectAudioClips();
            if (clips.Count == 0)
            {
                Debug.LogWarning(
                    "[Increment 018] No se encontraron AudioClip en las carpetas Music y SFX.");
                Selection.activeObject = config;
                return;
            }

            SerializedObject serializedConfig = new(config);

            AssignmentReport report = new();
            AssignClip(
                serializedConfig,
                "mainMenuMusic",
                clips,
                report,
                "african-inspiration",
                "main-menu",
                "menu",
                "ambient-morning",
                "safari");

            AssignClip(
                serializedConfig,
                "gameplayMusic",
                clips,
                report,
                "into-the-jungle",
                "song-of-jungle",
                "hurt-of-jungle",
                "gameplay",
                "jungle");

            AssignClip(
                serializedConfig,
                "buttonClick",
                clips,
                report,
                "button",
                "button-high",
                "buttonmap",
                "click");

            AssignClip(
                serializedConfig,
                "pieceCorrect",
                clips,
                report,
                "buttonmap",
                "piece-correct",
                "correct",
                "placed",
                "success");

            AssignClip(
                serializedConfig,
                "pieceIncorrect",
                clips,
                report,
                "close",
                "piece-incorrect",
                "incorrect",
                "error",
                "fail");

            AssignClip(
                serializedConfig,
                "puzzleCompleted",
                clips,
                report,
                "win",
                "puzzle-completed",
                "completed",
                "victory");

            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = config;

            Debug.Log(report.BuildMessage(), config);
        }
        public static void Validate()
        {
            bool valid = true;

            AudioConfig config = AssetDatabase.LoadAssetAtPath<AudioConfig>(AudioConfigPath);
            if (config == null)
            {
                valid = false;
                Debug.LogError($"[Increment 018] Falta {AudioConfigPath}.");
            }
            else
            {
                SerializedObject serializedConfig = new(config);
                string[] properties =
                {
                    "mainMenuMusic",
                    "gameplayMusic",
                    "buttonClick",
                    "pieceCorrect",
                    "pieceIncorrect",
                    "puzzleCompleted"
                };

                foreach (string propertyName in properties)
                {
                    SerializedProperty property =
                        serializedConfig.FindProperty(propertyName);

                    if (property == null || property.objectReferenceValue == null)
                    {
                        valid = false;
                        Debug.LogWarning(
                            $"[Increment 018] El campo {propertyName} no tiene AudioClip asignado.",
                            config);
                    }
                }
            }

            string[] requiredScripts =
            {
                "Assets/_Project/Code/Runtime/Haptics/HapticCue.cs",
                "Assets/_Project/Code/Runtime/Haptics/HapticService.cs"
            };

            foreach (string path in requiredScripts)
            {
                if (!File.Exists(path))
                {
                    valid = false;
                    Debug.LogError($"[Increment 018] Falta {path}.");
                }
            }

            if (valid)
            {
                Debug.Log("[Increment 018] Validación correcta.");
            }
        }

        private static List<AudioClipEntry> FindProjectAudioClips()
        {
            string[] existingFolders = AudioFolders
                .Where(AssetDatabase.IsValidFolder)
                .ToArray();

            if (existingFolders.Length == 0)
            {
                return new List<AudioClipEntry>();
            }

            return AssetDatabase.FindAssets("t:AudioClip", existingFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => new AudioClipEntry(
                    path,
                    AssetDatabase.LoadAssetAtPath<AudioClip>(path)))
                .Where(entry => entry.Clip != null)
                .OrderBy(entry => entry.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static void AssignClip(
            SerializedObject serializedConfig,
            string propertyName,
            IReadOnlyList<AudioClipEntry> clips,
            AssignmentReport report,
            params string[] preferredNames)
        {
            SerializedProperty property =
                serializedConfig.FindProperty(propertyName);

            if (property == null)
            {
                report.AddMissing(propertyName, "campo no encontrado");
                return;
            }

            AudioClipEntry? selected = SelectBestClip(clips, preferredNames);
            if (!selected.HasValue)
            {
                report.AddMissing(propertyName, "sin coincidencias");
                return;
            }

            property.objectReferenceValue = selected.Value.Clip;
            report.AddAssigned(propertyName, selected.Value.Path);
        }

        private static AudioClipEntry? SelectBestClip(
            IReadOnlyList<AudioClipEntry> clips,
            IReadOnlyList<string> preferredNames)
        {
            foreach (string preferred in preferredNames)
            {
                string normalizedPreferred = Normalize(preferred);

                AudioClipEntry? exact = clips
                    .Where(entry => Normalize(Path.GetFileNameWithoutExtension(entry.Path))
                        == normalizedPreferred)
                    .Cast<AudioClipEntry?>()
                    .FirstOrDefault();

                if (exact.HasValue)
                {
                    return exact;
                }
            }

            foreach (string preferred in preferredNames)
            {
                string normalizedPreferred = Normalize(preferred);

                AudioClipEntry? partial = clips
                    .Where(entry => Normalize(Path.GetFileNameWithoutExtension(entry.Path))
                        .Contains(normalizedPreferred))
                    .Cast<AudioClipEntry?>()
                    .FirstOrDefault();

                if (partial.HasValue)
                {
                    return partial;
                }
            }

            return null;
        }

        private static string Normalize(string value)
        {
            return value
                .Trim()
                .ToLowerInvariant()
                .Replace("_", "-")
                .Replace(" ", "-");
        }

        private readonly struct AudioClipEntry
        {
            public AudioClipEntry(string path, AudioClip clip)
            {
                Path = path;
                Clip = clip;
            }

            public string Path { get; }
            public AudioClip Clip { get; }
        }

        private sealed class AssignmentReport
        {
            private readonly List<string> assigned = new();
            private readonly List<string> missing = new();

            public void AddAssigned(string field, string path)
            {
                assigned.Add($"{field} ← {path}");
            }

            public void AddMissing(string field, string reason)
            {
                missing.Add($"{field}: {reason}");
            }

            public string BuildMessage()
            {
                string message =
                    "[Increment 018] Asignación automática de audio completada.";

                if (assigned.Count > 0)
                {
                    message += "\nAsignados:\n- " +
                        string.Join("\n- ", assigned);
                }

                if (missing.Count > 0)
                {
                    message += "\nPendientes:\n- " +
                        string.Join("\n- ", missing);
                }

                return message;
            }
        }
    }
}
