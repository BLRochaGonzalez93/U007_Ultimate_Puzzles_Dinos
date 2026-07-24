using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Persistence
{
    public static class ProgressSaveService
    {
        private const int CurrentVersion = 2;
        private const int TotalLevels = 60;
        private const string FileName = "progress.json";
        private const string BackupFileName = "progress.backup.json";

        public static string SavePath => Path.Combine(UnityEngine.Application.persistentDataPath, FileName);
        public static string BackupPath => Path.Combine(UnityEngine.Application.persistentDataPath, BackupFileName);

        public static ProgressData Load()
        {
            ProgressData data = TryLoad(SavePath);
            if (data != null)
            {
                return Sanitize(data);
            }

            data = TryLoad(BackupPath);
            return data != null ? Sanitize(data) : Sanitize(new ProgressData());
        }

        public static void Save(ProgressData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string directory = Path.GetDirectoryName(SavePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string temporaryPath = SavePath + ".tmp";
            string json = JsonUtility.ToJson(Sanitize(data), true);
            File.WriteAllText(temporaryPath, json);

            if (File.Exists(SavePath))
            {
                File.Copy(SavePath, BackupPath, true);
            }

            File.Copy(temporaryPath, SavePath, true);
            File.Delete(temporaryPath);
        }

        public static void Delete()
        {
            DeleteIfExists(SavePath);
            DeleteIfExists(BackupPath);
            DeleteIfExists(SavePath + ".tmp");
        }

        private static ProgressData TryLoad(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<ProgressData>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"No se pudo leer el progreso en '{path}': {exception.Message}");
                return null;
            }
        }

        private static ProgressData Sanitize(ProgressData data)
        {
            data ??= new ProgressData();
            data.standard ??= new ModeProgressData();
            data.logic ??= new ModeProgressData();
            data.mosaic ??= new ModeProgressData();
            data.completedLevels ??= new List<string>();

            if (data.version < CurrentVersion)
            {
                MigrateLegacyCompletedLevels(data);
            }

            SanitizeMode(data.standard);
            SanitizeMode(data.logic);
            SanitizeMode(data.mosaic);
            data.version = CurrentVersion;
            return data;
        }

        private static void MigrateLegacyCompletedLevels(ProgressData data)
        {
            foreach (string key in data.completedLevels)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                string[] parts = key.Split(':');
                if (parts.Length != 2 || !int.TryParse(parts[0], out int modeValue) || !int.TryParse(parts[1], out int levelId))
                {
                    continue;
                }

                ModeProgressData mode = modeValue switch
                {
                    0 => data.standard,
                    1 => data.logic,
                    2 => data.mosaic,
                    _ => null
                };

                if (mode != null && levelId >= 1 && levelId <= TotalLevels)
                {
                    SetBestStars(mode, levelId, 1);
                }
            }

            // No se migran los "highest unlocked" del sistema anterior porque
            // provenían del desbloqueo secuencial, comportamiento ya retirado.
            data.completedLevels.Clear();
            data.standardHighestUnlocked = 1;
            data.logicHighestUnlocked = 1;
            data.mosaicHighestUnlocked = 1;
        }

        private static void SanitizeMode(ModeProgressData mode)
        {
            mode.unlockedLevelIds ??= new List<int>();
            mode.levelStars ??= new List<LevelStarsData>();

            HashSet<int> validUnlocked = new();
            foreach (int levelId in mode.unlockedLevelIds)
            {
                if (levelId >= 1 && levelId <= TotalLevels)
                {
                    validUnlocked.Add(levelId);
                }
            }

            mode.unlockedLevelIds.Clear();
            mode.unlockedLevelIds.AddRange(validUnlocked);
            mode.unlockedLevelIds.Sort();

            Dictionary<int, int> bestStars = new();
            foreach (LevelStarsData entry in mode.levelStars)
            {
                if (entry == null || entry.levelId < 1 || entry.levelId > TotalLevels)
                {
                    continue;
                }

                int stars = Mathf.Clamp(entry.stars, 0, 4);
                if (!bestStars.TryGetValue(entry.levelId, out int current) || stars > current)
                {
                    bestStars[entry.levelId] = stars;
                }
            }

            mode.levelStars.Clear();
            foreach (KeyValuePair<int, int> pair in bestStars)
            {
                mode.levelStars.Add(new LevelStarsData { levelId = pair.Key, stars = pair.Value });
            }

            mode.levelStars.Sort((left, right) => left.levelId.CompareTo(right.levelId));
        }

        private static void SetBestStars(ModeProgressData mode, int levelId, int stars)
        {
            foreach (LevelStarsData entry in mode.levelStars)
            {
                if (entry.levelId == levelId)
                {
                    entry.stars = Mathf.Max(entry.stars, stars);
                    return;
                }
            }

            mode.levelStars.Add(new LevelStarsData { levelId = levelId, stars = stars });
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
