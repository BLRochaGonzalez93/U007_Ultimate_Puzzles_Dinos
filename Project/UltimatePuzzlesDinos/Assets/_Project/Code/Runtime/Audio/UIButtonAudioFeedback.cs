using UnityEngine;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.Audio
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class UIButtonAudioFeedback : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button ??= GetComponent<Button>();
            button.onClick.AddListener(PlayClick);
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlayClick);
            }
        }

        private static void PlayClick()
        {
            AudioService.PlaySfx(AudioCue.ButtonClick);
        }
    }
}
