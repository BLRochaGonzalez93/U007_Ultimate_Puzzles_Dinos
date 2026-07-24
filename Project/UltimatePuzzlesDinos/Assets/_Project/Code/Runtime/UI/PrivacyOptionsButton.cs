using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Monetization;

namespace VRMGames.UltimatePuzzlesDinos.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class PrivacyOptionsButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ShowPrivacyOptions);
            RefreshVisibility();
        }

        private void OnEnable()
        {
            RefreshVisibility();
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(ShowPrivacyOptions);
            }
        }

        public void RefreshVisibility()
        {
            gameObject.SetActive(
                AdsService.PrivacyOptionsRequired);
        }

        private void ShowPrivacyOptions()
        {
            AdsService.ShowPrivacyOptions(
                success =>
                {
                    if (!success)
                    {
                        Debug.LogWarning(
                            "[UMP] Privacy options were not shown.");
                    }

                    RefreshVisibility();
                });
        }
    }
}
