using System.Collections.Generic;
using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Content
{
    [CreateAssetMenu(fileName = "PuzzleCatalog", menuName = "VRM Games/Ultimate Puzzles Dinos/Puzzle Catalog")]
    public sealed class PuzzleCatalog : ScriptableObject
    {
        [SerializeField] private List<PuzzleDefinition> puzzles = new();

        public IReadOnlyList<PuzzleDefinition> Puzzles => puzzles;
        public int Count => puzzles?.Count ?? 0;

        public PuzzleDefinition GetByLevelNumber(int levelNumber)
        {
            if (puzzles == null || puzzles.Count == 0) return null;
            int index = Mathf.Clamp(levelNumber - 1, 0, puzzles.Count - 1);
            return puzzles[index];
        }
    }
}
