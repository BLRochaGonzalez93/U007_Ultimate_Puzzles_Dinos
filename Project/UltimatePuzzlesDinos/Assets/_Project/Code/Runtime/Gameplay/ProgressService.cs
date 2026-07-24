using System;
using System.Collections.Generic;
using UnityEngine;
using VRMGames.UltimatePuzzlesDinos.Configuration;
using VRMGames.UltimatePuzzlesDinos.Persistence;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay
{
    public static class ProgressService
    {
        private const int TotalLevels = PuzzleLevelCatalog.TotalLevelCount;
        private static ProgressData data;
        private static EditionConfig editionConfig;

        public static event Action ProgressChanged;

        public static bool RewardedUnlocksEnabled =>
            editionConfig != null &&
            editionConfig.RewardedUnlocksEnabled &&
            !editionConfig.AllContentUnlocked;

        public static void Initialize(EditionConfig config)
        {
            editionConfig = config;
            data = ProgressSaveService.Load();
            ApplyEditionRules();
            Save();
        }

        public static bool IsLevelUnlocked(PuzzleMode mode, int levelId)
        {
            EnsureInitialized();
            int normalizedLevel = Mathf.Clamp(levelId, 1, TotalLevels);
            if (editionConfig != null && editionConfig.AllContentUnlocked)
            {
                return true;
            }

            return GetModeData(mode).unlockedLevelIds.Contains(normalizedLevel);
        }

        public static bool CanUnlockWithReward(PuzzleMode mode, int levelId)
        {
            return RewardedUnlocksEnabled && !IsLevelUnlocked(mode, levelId);
        }

        public static bool UnlockLevelWithReward(PuzzleMode mode, int levelId)
        {
            EnsureInitialized();
            if (!CanUnlockWithReward(mode, levelId))
            {
                return false;
            }

            int normalizedLevel = Mathf.Clamp(levelId, 1, TotalLevels);
            ModeProgressData modeData = GetModeData(mode);
            if (modeData.unlockedLevelIds.Contains(normalizedLevel))
            {
                return false;
            }

            modeData.unlockedLevelIds.Add(normalizedLevel);
            modeData.unlockedLevelIds.Sort();
            SaveAndNotify();
            return true;
        }

        public static int GetStars(PuzzleMode mode, int levelId)
        {
            EnsureInitialized();
            int normalizedLevel = Mathf.Clamp(levelId, 1, TotalLevels);
            foreach (LevelStarsData entry in GetModeData(mode).levelStars)
            {
                if (entry.levelId == normalizedLevel)
                {
                    return Mathf.Clamp(entry.stars, 0, 4);
                }
            }

            return 0;
        }

        public static bool RecordCompletion(PuzzleMode mode, int levelId, PuzzleDifficulty difficulty)
        {
            EnsureInitialized();
            int normalizedLevel = Mathf.Clamp(levelId, 1, TotalLevels);
            int earnedStars = Mathf.Clamp((int)difficulty + 1, 1, 4);
            ModeProgressData modeData = GetModeData(mode);

            foreach (LevelStarsData entry in modeData.levelStars)
            {
                if (entry.levelId != normalizedLevel)
                {
                    continue;
                }

                if (earnedStars <= entry.stars)
                {
                    return false;
                }

                entry.stars = earnedStars;
                SaveAndNotify();
                return true;
            }

            modeData.levelStars.Add(new LevelStarsData
            {
                levelId = normalizedLevel,
                stars = earnedStars
            });
            modeData.levelStars.Sort((left, right) => left.levelId.CompareTo(right.levelId));
            SaveAndNotify();
            return true;
        }

        public static void ResetProgress()
        {
            ProgressSaveService.Delete();
            data = new ProgressData();
            ApplyEditionRules();
            SaveAndNotify();
        }

        private static void EnsureInitialized()
        {
            if (data != null)
            {
                return;
            }

            data = ProgressSaveService.Load();
            ApplyEditionRules();
        }

        private static void ApplyEditionRules()
        {
            int initiallyUnlocked = editionConfig != null
                ? Mathf.Clamp(editionConfig.InitiallyUnlockedPuzzleCount, 0, TotalLevels)
                : PuzzleLevelCatalog.InitialUnlockedLevels;

            // Premium grants runtime access through IsLevelUnlocked().
            // It must never persist all 60 levels into the Free save data.
            if (editionConfig != null && editionConfig.AllContentUnlocked)
            {
                return;
            }

            ApplyInitialUnlocks(data.standard, initiallyUnlocked);
            ApplyInitialUnlocks(data.logic, initiallyUnlocked);
            ApplyInitialUnlocks(data.mosaic, initiallyUnlocked);
        }

        private static void ApplyInitialUnlocks(ModeProgressData modeData, int count)
        {
            modeData.unlockedLevelIds ??= new List<int>();
            modeData.levelStars ??= new List<LevelStarsData>();
            for (int levelId = 1; levelId <= count; levelId++)
            {
                if (!modeData.unlockedLevelIds.Contains(levelId))
                {
                    modeData.unlockedLevelIds.Add(levelId);
                }
            }

            modeData.unlockedLevelIds.Sort();
        }

        private static ModeProgressData GetModeData(PuzzleMode mode)
        {
            return mode switch
            {
                PuzzleMode.Standard => data.standard,
                PuzzleMode.Logic => data.logic,
                PuzzleMode.Mosaic => data.mosaic,
                _ => data.standard
            };
        }

        private static void SaveAndNotify()
        {
            Save();
            ProgressChanged?.Invoke();
        }

        private static void Save()
        {
            ProgressSaveService.Save(data);
        }
    }
}
