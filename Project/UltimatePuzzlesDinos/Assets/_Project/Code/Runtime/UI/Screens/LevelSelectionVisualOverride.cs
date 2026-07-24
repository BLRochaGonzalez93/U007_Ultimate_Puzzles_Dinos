using UnityEngine;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class LevelSelectionVisualOverride : MonoBehaviour
    {
        private float nextRefreshTime;

        private void OnEnable()
        {
            Apply();
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime < nextRefreshTime)
            {
                return;
            }

            nextRefreshTime = Time.unscaledTime + 0.5f;
            Apply();
        }

        private void Apply()
        {
            RectTransform[] rects = GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform rect in rects)
            {
                if (rect == null)
                {
                    continue;
                }

                if (rect.name == "PreviewPlaceholder")
                {
                    Text placeholder = rect.GetComponent<Text>();
                    if (placeholder != null)
                    {
                        placeholder.text = string.Empty;
                        placeholder.enabled = false;
                    }

                    rect.gameObject.SetActive(false);
                }
                else if (rect.name == "LockIndicator")
                {
                    Text lockText = rect.GetComponent<Text>();
                    if (lockText != null)
                    {
                        lockText.text = string.Empty;
                        lockText.enabled = false;
                    }
                }
                else if (rect.name == "Preview")
                {
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = new Vector2(18f, 18f);
                    rect.offsetMax = new Vector2(-18f, -18f);
                    Image image = rect.GetComponent<Image>();
                    if (image != null)
                    {
                        image.preserveAspect = true;
                    }
                }
            }
        }
    }
}
