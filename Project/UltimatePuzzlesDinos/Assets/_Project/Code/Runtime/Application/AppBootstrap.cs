using UnityEngine;
using UnityEngine.SceneManagement;
using VRMGames.UltimatePuzzlesDinos.Audio;
using VRMGames.UltimatePuzzlesDinos.Configuration;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.Monetization;
using VRMGames.UltimatePuzzlesDinos.Scenes;

namespace VRMGames.UltimatePuzzlesDinos.Application
{
    [DisallowMultipleComponent]
    public sealed class AppBootstrap : MonoBehaviour
    {
        [SerializeField] private EditionConfig editionConfig;
        [SerializeField] private bool loadMainMenuOnStart = true;

        private static AppBootstrap instance;

        public static AppBootstrap Instance => instance;
        public EditionConfig EditionConfig => editionConfig;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            ProgressService.Initialize(editionConfig);
            AdsService.Initialize(editionConfig);
            AudioService.EnsureExists();
        }

        private void Start()
        {
            if (!loadMainMenuOnStart)
            {
                return;
            }

            if (SceneManager.GetActiveScene().name == SceneNames.Bootstrap)
            {
                SceneManager.LoadScene(SceneNames.MainMenu);
            }
        }
    }
}
