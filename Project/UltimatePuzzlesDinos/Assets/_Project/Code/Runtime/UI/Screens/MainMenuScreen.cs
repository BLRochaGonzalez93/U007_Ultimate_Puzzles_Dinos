using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.Navigation;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] private Button puzzleButton;
        [SerializeField] private Button puzzleLogicButton;
        [SerializeField] private Button mosaicButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private SettingsPanel settingsPanel;

        private void OnEnable()
        {
            puzzleButton?.onClick.AddListener(OpenStandardPuzzle);
            puzzleLogicButton?.onClick.AddListener(OpenLogicPuzzle);
            mosaicButton?.onClick.AddListener(OpenMosaic);
            settingsButton?.onClick.AddListener(OpenSettings);
            quitButton?.onClick.AddListener(SceneNavigator.QuitApplication);
        }

        private void OnDisable()
        {
            puzzleButton?.onClick.RemoveListener(OpenStandardPuzzle);
            puzzleLogicButton?.onClick.RemoveListener(OpenLogicPuzzle);
            mosaicButton?.onClick.RemoveListener(OpenMosaic);
            settingsButton?.onClick.RemoveListener(OpenSettings);
            quitButton?.onClick.RemoveListener(SceneNavigator.QuitApplication);
        }

        private static void OpenStandardPuzzle() => OpenMode(PuzzleMode.Standard);
        private static void OpenLogicPuzzle() => OpenMode(PuzzleMode.Logic);
        private static void OpenMosaic() => OpenMode(PuzzleMode.Mosaic);

        private static void OpenMode(PuzzleMode mode)
        {
            PuzzleSession.SelectMode(mode);
            SceneNavigator.OpenLevelSelection();
        }

        private void OpenSettings() => settingsPanel?.Show();
    }
}
