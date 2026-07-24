using System.Collections.Generic;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay
{
    public readonly struct PuzzleDifficultyInfo
    {
        public PuzzleDifficultyInfo(PuzzleDifficulty difficulty, string displayName, int columns, int rows)
        {
            Difficulty = difficulty;
            DisplayName = displayName;
            Columns = columns;
            Rows = rows;
        }

        public PuzzleDifficulty Difficulty { get; }
        public string DisplayName { get; }
        public int Columns { get; }
        public int Rows { get; }
        public int PieceCount => Columns * Rows;
    }

    public static class PuzzleDifficultyCatalog
    {
        private static readonly PuzzleDifficultyInfo[] Difficulties =
        {
            new(PuzzleDifficulty.Easy, "FACIL", 3, 3),
            new(PuzzleDifficulty.Normal, "NORMAL", 4, 4),
            new(PuzzleDifficulty.Hard, "DIFICIL", 6, 6),
            new(PuzzleDifficulty.Expert, "EXPERTO", 8, 8)
        };

        public static IReadOnlyList<PuzzleDifficultyInfo> GetDifficulties() => Difficulties;

        public static PuzzleDifficultyInfo Get(PuzzleDifficulty difficulty)
        {
            foreach (PuzzleDifficultyInfo item in Difficulties)
            {
                if (item.Difficulty == difficulty)
                {
                    return item;
                }
            }

            return Difficulties[0];
        }
    }
}
