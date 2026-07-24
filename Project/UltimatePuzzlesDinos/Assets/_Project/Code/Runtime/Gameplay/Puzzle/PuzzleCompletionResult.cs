namespace VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle
{
    public readonly struct PuzzleCompletionResult
    {
        public PuzzleCompletionResult(
            PuzzleMode mode,
            int levelId,
            PuzzleDifficulty difficulty,
            int earnedStars,
            int previousBestStars,
            int bestStars,
            bool improvedBest,
            int moves,
            float elapsedSeconds)
        {
            Mode = mode;
            LevelId = levelId;
            Difficulty = difficulty;
            EarnedStars = earnedStars;
            PreviousBestStars = previousBestStars;
            BestStars = bestStars;
            ImprovedBest = improvedBest;
            Moves = moves;
            ElapsedSeconds = elapsedSeconds;
        }

        public PuzzleMode Mode { get; }
        public int LevelId { get; }
        public PuzzleDifficulty Difficulty { get; }
        public int EarnedStars { get; }
        public int PreviousBestStars { get; }
        public int BestStars { get; }
        public bool ImprovedBest { get; }
        public int Moves { get; }
        public float ElapsedSeconds { get; }
    }
}
