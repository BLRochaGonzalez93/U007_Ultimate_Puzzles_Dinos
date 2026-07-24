using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Scenes;
using VRMGames.UltimatePuzzlesDinos.Settings;

namespace VRMGames.UltimatePuzzlesDinos.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioService : MonoBehaviour
    {
        private const string ConfigResourcePath = "Audio/AudioConfig";
        private const float ButtonScanInterval = 0.35f;

        private static AudioService instance;

        private AudioConfig config;
        private AudioSource musicSource;
        private AudioSource sfxSource;
        private Coroutine buttonBindingRoutine;

        public static bool IsAvailable => instance != null;

        public static void EnsureExists()
        {
            if (instance != null)
            {
                return;
            }

            GameObject serviceObject = new("AudioService");
            instance = serviceObject.AddComponent<AudioService>();
            DontDestroyOnLoad(serviceObject);
        }

        public static void PlaySfx(AudioCue cue)
        {
            if (instance == null)
            {
                EnsureExists();
            }

            instance.PlayCueInternal(cue);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            config = Resources.Load<AudioConfig>(ConfigResourcePath);
            musicSource = CreateSource("MusicSource", loop: true);
            sfxSource = CreateSource("SfxSource", loop: false);

            ApplyVolumes();
            SettingsService.Changed += ApplyVolumes;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            HandleScene(SceneManager.GetActiveScene());
        }

        private void OnDestroy()
        {
            if (instance != this)
            {
                return;
            }

            SettingsService.Changed -= ApplyVolumes;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }

        private AudioSource CreateSource(string objectName, bool loop)
        {
            GameObject sourceObject = new(objectName);
            sourceObject.transform.SetParent(transform, false);

            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            source.ignoreListenerPause = false;
            return source;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            HandleScene(scene);
        }

        private void HandleScene(Scene scene)
        {
            PlayMusicForScene(scene.name);

            if (buttonBindingRoutine != null)
            {
                StopCoroutine(buttonBindingRoutine);
            }

            buttonBindingRoutine = StartCoroutine(BindButtonsForShortPeriod());
        }

        private void PlayMusicForScene(string sceneName)
        {
            if (config == null)
            {
                return;
            }

            AudioClip requestedClip = sceneName switch
            {
                SceneNames.MainMenu => config.MainMenuMusic,
                SceneNames.LevelSelection => config.MainMenuMusic,
                SceneNames.DifficultySelection => config.MainMenuMusic,
                SceneNames.Gameplay => config.GameplayMusic,
                _ => null
            };

            if (requestedClip == null)
            {
                musicSource.Stop();
                musicSource.clip = null;
                return;
            }

            if (musicSource.clip == requestedClip && musicSource.isPlaying)
            {
                return;
            }

            musicSource.Stop();
            musicSource.clip = requestedClip;
            musicSource.Play();
        }

        private IEnumerator BindButtonsForShortPeriod()
        {
            const int scanCount = 10;

            for (int scan = 0; scan < scanCount; scan++)
            {
                BindCurrentButtons();
                yield return new WaitForSecondsRealtime(ButtonScanInterval);
            }

            buttonBindingRoutine = null;
        }

        private static void BindCurrentButtons()
        {
            Button[] buttons = FindObjectsByType<Button>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (Button button in buttons)
            {
                if (button == null || button.GetComponent<UIButtonAudioFeedback>() != null)
                {
                    continue;
                }

                button.gameObject.AddComponent<UIButtonAudioFeedback>();
            }
        }

        private void ApplyVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = SettingsService.MusicVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = SettingsService.SfxVolume;
            }
        }

        private void PlayCueInternal(AudioCue cue)
        {
            if (config == null || sfxSource == null)
            {
                return;
            }

            AudioClip clip = config.GetCue(cue);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}
