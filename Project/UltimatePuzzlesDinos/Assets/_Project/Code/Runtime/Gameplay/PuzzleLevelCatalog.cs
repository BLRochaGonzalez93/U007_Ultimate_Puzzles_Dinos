using System.Collections.Generic;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay
{
    public readonly struct PuzzleLevelInfo
    {
        public PuzzleLevelInfo(int id, string displayName, bool unlocked, int stars)
        {
            Id = id;
            DisplayName = displayName;
            Unlocked = unlocked;
            Stars = stars;
        }

        public int Id { get; }
        public string DisplayName { get; }
        public bool Unlocked { get; }
        public int Stars { get; }
    }

    public static class PuzzleLevelCatalog
    {
        public const int TotalLevelCount = 60;
        public const int InitialUnlockedLevels = 3;

        public static IReadOnlyList<PuzzleLevelInfo> GetLevels()
        {
            List<PuzzleLevelInfo> levels = new(TotalLevelCount);
            for (int index = 0; index < TotalLevelCount; index++)
            {
                int id = index + 1;
                levels.Add(CreateInfo(id));
            }

            return levels;
        }

        public static PuzzleLevelInfo GetById(int id)
        {
            int normalizedId = id >= 1 && id <= TotalLevelCount ? id : 1;
            return CreateInfo(normalizedId);
        }

        private static PuzzleLevelInfo CreateInfo(int id)
        {
            return new PuzzleLevelInfo(
                id,
                $"NIVEL {id:00}",
                ProgressService.IsLevelUnlocked(PuzzleSession.SelectedMode, id),
                ProgressService.GetStars(PuzzleSession.SelectedMode, id));
        }
    }
}
