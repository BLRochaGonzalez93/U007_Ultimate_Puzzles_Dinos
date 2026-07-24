using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.Audio
{
    [CreateAssetMenu(
        fileName = "AudioConfig",
        menuName = "VRM Games/Ultimate Puzzles Dinos/Audio Configuration")]
    public sealed class AudioConfig : ScriptableObject
    {
        [Header("Music")]
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("Sound effects")]
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip pieceCorrect;
        [SerializeField] private AudioClip pieceIncorrect;
        [SerializeField] private AudioClip puzzleCompleted;

        public AudioClip MainMenuMusic => mainMenuMusic;
        public AudioClip GameplayMusic => gameplayMusic;

        public AudioClip GetCue(AudioCue cue)
        {
            return cue switch
            {
                AudioCue.ButtonClick => buttonClick,
                AudioCue.PieceCorrect => pieceCorrect,
                AudioCue.PieceIncorrect => pieceIncorrect,
                AudioCue.PuzzleCompleted => puzzleCompleted,
                _ => null
            };
        }
    }
}
