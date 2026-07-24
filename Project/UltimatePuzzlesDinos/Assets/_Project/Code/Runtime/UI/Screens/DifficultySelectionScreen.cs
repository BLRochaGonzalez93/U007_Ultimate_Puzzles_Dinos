using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.Navigation;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class DifficultySelectionScreen : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Text modeLabel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private List<Button> difficultyButtons = new();
        [SerializeField] private List<Text> difficultyLabels = new();
        [SerializeField] private List<Text> gridLabels = new();

        private readonly List<UnityEngine.Events.UnityAction> listeners = new();

        private void OnEnable()
        {
            backButton?.onClick.AddListener(SceneNavigator.OpenLevelSelection);
            Refresh();
        }

        private void OnDisable()
        {
            backButton?.onClick.RemoveListener(SceneNavigator.OpenLevelSelection);
            RemoveListeners();
        }

        private void Refresh()
        {
            if (modeLabel != null) modeLabel.text = PuzzleSession.GetModeDisplayName();
            if (levelLabel != null) levelLabel.text = PuzzleSession.GetLevelDisplayName();

            RemoveListeners();
            IReadOnlyList<PuzzleDifficultyInfo> difficulties = PuzzleDifficultyCatalog.GetDifficulties();
            int count = Mathf.Min(difficulties.Count, difficultyButtons.Count);

            for (int index = 0; index < count; index++)
            {
                PuzzleDifficultyInfo info = difficulties[index];
                if (index < difficultyLabels.Count && difficultyLabels[index] != null)
                {
                    difficultyLabels[index].text = info.DisplayName;
                }

                if (index < gridLabels.Count && gridLabels[index] != null)
                {
                    gridLabels[index].text = $"{info.Columns} x {info.Rows}\n{info.PieceCount} PIEZAS";
                }

                Button button = difficultyButtons[index];
                if (button == null)
                {
                    listeners.Add(null);
                    continue;
                }

                PuzzleDifficulty capturedDifficulty = info.Difficulty;
                UnityEngine.Events.UnityAction listener = () => OpenGameplay(capturedDifficulty);
                listeners.Add(listener);
                button.onClick.AddListener(listener);
            }
        }

        private static void OpenGameplay(PuzzleDifficulty difficulty)
        {
            PuzzleSession.SelectDifficulty(difficulty);
            SceneNavigator.OpenGameplay();
        }

        private void RemoveListeners()
        {
            int count = Mathf.Min(difficultyButtons.Count, listeners.Count);
            for (int index = 0; index < count; index++)
            {
                if (difficultyButtons[index] != null && listeners[index] != null)
                {
                    difficultyButtons[index].onClick.RemoveListener(listeners[index]);
                }
            }

            listeners.Clear();
        }
    }
}
