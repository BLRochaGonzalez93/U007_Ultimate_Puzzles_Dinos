namespace VRMGames.UltimatePuzzlesDinos.Gameplay
{
    public static class PuzzleSession
    {
        public static PuzzleMode SelectedMode { get; private set; } = PuzzleMode.Standard;
        public static int SelectedLevelId { get; private set; } = 1;
        public static PuzzleDifficulty SelectedDifficulty { get; private set; } = PuzzleDifficulty.Easy;

        public static void SelectMode(PuzzleMode mode)
        {
            SelectedMode = mode;
            SelectedLevelId = 1;
            SelectedDifficulty = PuzzleDifficulty.Easy;
        }

        public static void SelectLevel(int levelId)
        {
            PuzzleLevelInfo level = PuzzleLevelCatalog.GetById(levelId);
            if (level.Unlocked)
            {
                SelectedLevelId = level.Id;
                SelectedDifficulty = PuzzleDifficulty.Easy;
            }
        }

        public static void SelectDifficulty(PuzzleDifficulty difficulty)
        {
            SelectedDifficulty = difficulty;
        }

        public static string GetModeDisplayName()
        {
            return SelectedMode switch
            {
                PuzzleMode.Standard => "PUZZLE",
                PuzzleMode.Logic => "PUZZLE LOGIC",
                PuzzleMode.Mosaic => "MOSAIC",
                _ => "PUZZLE"
            };
        }

        public static string GetLevelDisplayName() => $"NIVEL {SelectedLevelId:00}";
        public static string GetDifficultyDisplayName() => PuzzleDifficultyCatalog.Get(SelectedDifficulty).DisplayName;

        public static string GetGridDisplayName()
        {
            PuzzleDifficultyInfo difficulty = PuzzleDifficultyCatalog.Get(SelectedDifficulty);
            return $"{difficulty.Columns} x {difficulty.Rows}  ·  {difficulty.PieceCount} PIEZAS";
        }
    }
}
