using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Content
{
    [CreateAssetMenu(fileName = "Puzzle_", menuName = "VRM Games/Ultimate Puzzles Dinos/Puzzle Definition")]
    public sealed class PuzzleDefinition : ScriptableObject
    {
        [SerializeField] private string id = "puzzle_001";
        [SerializeField] private string displayName = "Dinosaurio 01";
        [SerializeField] private Sprite image;
        [SerializeField] private Color fallbackColor = new(0.72f, 0.34f, 0.12f, 1f);

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Image => image;
        public Color FallbackColor => fallbackColor;
    }
}
