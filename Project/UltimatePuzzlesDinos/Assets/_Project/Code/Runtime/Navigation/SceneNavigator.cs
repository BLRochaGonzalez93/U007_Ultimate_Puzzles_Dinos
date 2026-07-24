using UnityEngine;
using UnityEngine.SceneManagement;
using VRMGames.UltimatePuzzlesDinos.Scenes;

namespace VRMGames.UltimatePuzzlesDinos.Navigation
{
    public static class SceneNavigator
    {
        public static void OpenMainMenu() => Load(SceneNames.MainMenu);
        public static void OpenLevelSelection() => Load(SceneNames.LevelSelection);
        public static void OpenDifficultySelection() => Load(SceneNames.DifficultySelection);
        public static void OpenGameplay() => Load(SceneNames.Gameplay);

        public static void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.IsValid())
            {
                Load(currentScene.name);
            }
        }

        public static void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private static void Load(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[Ultimate Puzzles Dinos] Cannot load an empty scene name.");
                return;
            }

            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
