using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle;
using VRMGames.UltimatePuzzlesDinos.Navigation;
using VRMGames.UltimatePuzzlesDinos.Monetization;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class GameplayScreen : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Text modeLabel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private Text difficultyLabel;
        [SerializeField] private Text gridLabel;
        [SerializeField] private PuzzleBoardController boardController;
        [SerializeField] private PuzzleLogicBoardController logicBoardController;
        [SerializeField] private MosaicBoardController mosaicBoardController;
        [SerializeField] private GameObject unsupportedModePanel;
        [SerializeField] private Text unsupportedModeLabel;

        private PuzzleResultPanel resultPanel;

        private void OnEnable()
        {
            EnsureModeControllers();

            backButton?.onClick.AddListener(
                SceneNavigator.OpenDifficultySelection);
            restartButton?.onClick.AddListener(RestartGameplay);

            if (boardController != null)
            {
                boardController.PuzzleCompleted += HandlePuzzleCompleted;
            }

            if (logicBoardController != null)
            {
                logicBoardController.PuzzleCompleted += HandlePuzzleCompleted;
            }

            if (mosaicBoardController != null)
            {
                mosaicBoardController.PuzzleCompleted += HandlePuzzleCompleted;
            }

            EnsureResultPanel();
            resultPanel.Hide();
            Refresh();
        }

        private void OnDisable()
        {
            backButton?.onClick.RemoveListener(
                SceneNavigator.OpenDifficultySelection);
            restartButton?.onClick.RemoveListener(RestartGameplay);

            if (boardController != null)
            {
                boardController.PuzzleCompleted -= HandlePuzzleCompleted;
            }

            if (logicBoardController != null)
            {
                logicBoardController.PuzzleCompleted -= HandlePuzzleCompleted;
            }

            if (mosaicBoardController != null)
            {
                mosaicBoardController.PuzzleCompleted -= HandlePuzzleCompleted;
            }
        }

        private void Refresh()
        {
            if (modeLabel != null)
            {
                modeLabel.text = PuzzleSession.GetModeDisplayName();
            }

            if (levelLabel != null)
            {
                levelLabel.text = PuzzleSession.GetLevelDisplayName();
            }

            if (difficultyLabel != null)
            {
                difficultyLabel.text =
                    PuzzleSession.GetDifficultyDisplayName();
            }

            if (gridLabel != null)
            {
                gridLabel.text = PuzzleSession.GetGridDisplayName();
            }

            bool standardMode =
                PuzzleSession.SelectedMode == PuzzleMode.Standard;
            bool logicMode =
                PuzzleSession.SelectedMode == PuzzleMode.Logic;
            bool mosaicMode =
                PuzzleSession.SelectedMode == PuzzleMode.Mosaic;

            if (boardController != null)
            {
                boardController.enabled = standardMode;
            }

            if (logicBoardController != null)
            {
                logicBoardController.enabled = logicMode;
            }

            if (mosaicBoardController != null)
            {
                mosaicBoardController.enabled = mosaicMode;
            }

            if (unsupportedModePanel != null)
            {
                unsupportedModePanel.SetActive(false);
            }
        }

        private void RestartGameplay()
        {
            resultPanel?.Hide();

            switch (PuzzleSession.SelectedMode)
            {
                case PuzzleMode.Standard when boardController != null:
                    boardController.RestartBoard();
                    break;

                case PuzzleMode.Logic when logicBoardController != null:
                    logicBoardController.RestartBoard();
                    break;

                case PuzzleMode.Mosaic when mosaicBoardController != null:
                    mosaicBoardController.RestartBoard();
                    break;

                default:
                    SceneNavigator.ReloadCurrentScene();
                    break;
            }
        }

        private void HandlePuzzleCompleted(PuzzleCompletionResult result)
        {
            EnsureResultPanel();
            resultPanel.Show(result);
            AdsService.RegisterPuzzleCompletion();
        }

        private void EnsureModeControllers()
        {
            if (boardController == null)
            {
                return;
            }

            if (logicBoardController == null)
            {
                logicBoardController =
                    boardController.GetComponent
                        <PuzzleLogicBoardController>();

                if (logicBoardController == null)
                {
                    logicBoardController =
                        boardController.gameObject
                            .AddComponent
                                <PuzzleLogicBoardController>();
                }

                logicBoardController.Initialize(boardController);
            }

            if (mosaicBoardController == null)
            {
                mosaicBoardController =
                    boardController.GetComponent
                        <MosaicBoardController>();

                if (mosaicBoardController == null)
                {
                    mosaicBoardController =
                        boardController.gameObject
                            .AddComponent<MosaicBoardController>();
                }

                mosaicBoardController.Initialize(boardController);
            }
        }

        private void EnsureResultPanel()
        {
            if (resultPanel != null)
            {
                return;
            }

            resultPanel = GetComponent<PuzzleResultPanel>();

            if (resultPanel == null)
            {
                resultPanel =
                    gameObject.AddComponent<PuzzleResultPanel>();
            }

            resultPanel.Initialize(
                RestartGameplay,
                SceneNavigator.OpenLevelSelection);
        }
    }
}
