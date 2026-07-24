using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Settings;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Text musicValueLabel;
        [SerializeField] private Text sfxValueLabel;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button resetButton;

        public void Show()
        {
            if (panelRoot == null)
            {
                return;
            }

            LoadCurrentValues();
            panelRoot.SetActive(true);
        }

        public void Hide()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        private void Awake()
        {
            ConfigureControls();
            LoadCurrentValues();
        }

        private void OnEnable()
        {
            if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            if (vibrationToggle != null) vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
            if (closeButton != null) closeButton.onClick.AddListener(Hide);
            if (resetButton != null) resetButton.onClick.AddListener(ResetSettings);
        }

        private void OnDisable()
        {
            if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            if (vibrationToggle != null) vibrationToggle.onValueChanged.RemoveListener(OnVibrationChanged);
            if (closeButton != null) closeButton.onClick.RemoveListener(Hide);
            if (resetButton != null) resetButton.onClick.RemoveListener(ResetSettings);
        }

        private void ConfigureControls()
        {
            ConfigureSlider(musicSlider);
            ConfigureSlider(sfxSlider);
        }

        private static void ConfigureSlider(Slider slider)
        {
            if (slider == null) return;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
        }

        private void LoadCurrentValues()
        {
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(SettingsService.MusicVolume);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(SettingsService.SfxVolume);
            if (vibrationToggle != null) vibrationToggle.SetIsOnWithoutNotify(SettingsService.VibrationEnabled);
            RefreshLabels();
        }

        private void OnMusicVolumeChanged(float value)
        {
            SettingsService.MusicVolume = value;
            RefreshLabels();
        }

        private void OnSfxVolumeChanged(float value)
        {
            SettingsService.SfxVolume = value;
            RefreshLabels();
        }

        private void OnVibrationChanged(bool enabled)
        {
            SettingsService.VibrationEnabled = enabled;
        }

        private void ResetSettings()
        {
            SettingsService.ResetToDefaults();
            LoadCurrentValues();
        }

        private void RefreshLabels()
        {
            if (musicValueLabel != null)
            {
                musicValueLabel.text = $"{Mathf.RoundToInt(SettingsService.MusicVolume * 100f)}%";
            }

            if (sfxValueLabel != null)
            {
                sfxValueLabel.text = $"{Mathf.RoundToInt(SettingsService.SfxVolume * 100f)}%";
            }
        }
    }
}
