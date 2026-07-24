using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class SettingsPanelVisualLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRoot;

        private void Awake()
        {
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        public void Apply()
        {
            RectTransform panel = panelRoot != null ? panelRoot : transform as RectTransform;
            if (panel == null)
            {
                return;
            }

            panel.anchorMin = new Vector2(0.5f, 0.5f);
            panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.pivot = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(760f, 620f);
            panel.anchoredPosition = Vector2.zero;

            RectTransform title = Find(panel, "Title");
            if (title != null)
            {
                title.anchorMin = new Vector2(0.5f, 1f);
                title.anchorMax = new Vector2(0.5f, 1f);
                title.pivot = new Vector2(0.5f, 1f);
                title.sizeDelta = new Vector2(520f, 56f);
                title.anchoredPosition = new Vector2(0f, -26f);
            }

            List<RectTransform> rows = new();
            CollectRows(panel, rows);
            rows.Sort((a, b) => ScoreRowName(a.name).CompareTo(ScoreRowName(b.name)));
            for (int i = 0; i < rows.Count; i++)
            {
                LayoutRow(rows[i], i);
            }

            RectTransform closeButton = Find(panel, "CloseButton");
            RectTransform resetButton = Find(panel, "ResetButton");

            if (resetButton != null)
            {
                PlaceBottomButton(resetButton, new Vector2(-140f, 42f), new Vector2(220f, 74f));
            }

            if (closeButton != null)
            {
                PlaceBottomButton(closeButton, new Vector2(140f, 42f), new Vector2(220f, 74f));
            }
        }

        private static void LayoutRow(RectTransform row, int index)
        {
            row.anchorMin = new Vector2(0.5f, 1f);
            row.anchorMax = new Vector2(0.5f, 1f);
            row.pivot = new Vector2(0.5f, 1f);
            row.sizeDelta = new Vector2(660f, 94f);
            row.anchoredPosition = new Vector2(0f, -108f - index * 126f);

            Text[] labels = row.GetComponentsInChildren<Text>(true);
            foreach (Text label in labels)
            {
                RectTransform labelRect = label.transform as RectTransform;
                if (labelRect == null)
                {
                    continue;
                }

                if (label.name == "Value")
                {
                    labelRect.anchorMin = new Vector2(1f, 0.5f);
                    labelRect.anchorMax = new Vector2(1f, 0.5f);
                    labelRect.pivot = new Vector2(1f, 0.5f);
                    labelRect.sizeDelta = new Vector2(86f, 42f);
                    labelRect.anchoredPosition = new Vector2(-8f, 0f);
                    label.alignment = TextAnchor.MiddleRight;
                }
                else if (label.name == "Label")
                {
                    labelRect.anchorMin = new Vector2(0f, 0.5f);
                    labelRect.anchorMax = new Vector2(0f, 0.5f);
                    labelRect.pivot = new Vector2(0f, 0.5f);
                    labelRect.sizeDelta = new Vector2(220f, 48f);
                    labelRect.anchoredPosition = new Vector2(16f, 0f);
                    label.alignment = TextAnchor.MiddleLeft;
                    label.resizeTextForBestFit = true;
                    label.resizeTextMinSize = 16;
                    label.resizeTextMaxSize = 26;
                }
            }

            Slider slider = row.GetComponentInChildren<Slider>(true);
            if (slider != null)
            {
                RectTransform sliderRect = slider.transform as RectTransform;
                if (sliderRect != null)
                {
                    sliderRect.anchorMin = new Vector2(0f, 0.5f);
                    sliderRect.anchorMax = new Vector2(0f, 0.5f);
                    sliderRect.pivot = new Vector2(0f, 0.5f);
                    sliderRect.sizeDelta = new Vector2(320f, 42f);
                    sliderRect.anchoredPosition = new Vector2(250f, 0f);
                }
            }

            Toggle toggle = row.GetComponentInChildren<Toggle>(true);
            if (toggle != null)
            {
                RectTransform toggleRect = toggle.transform as RectTransform;
                if (toggleRect != null)
                {
                    toggleRect.anchorMin = new Vector2(1f, 0.5f);
                    toggleRect.anchorMax = new Vector2(1f, 0.5f);
                    toggleRect.pivot = new Vector2(1f, 0.5f);
                    toggleRect.sizeDelta = new Vector2(84f, 84f);
                    toggleRect.anchoredPosition = new Vector2(-14f, 0f);
                }
            }
        }

        private static void PlaceBottomButton(RectTransform rect, Vector2 pos, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;
        }

        private static int ScoreRowName(string name)
        {
            string lowered = name.ToLowerInvariant();
            if (lowered.Contains("music") || lowered.Contains("músi") || lowered.Contains("musi")) return 0;
            if (lowered.Contains("efect")) return 1;
            if (lowered.Contains("vibra")) return 2;
            return 100;
        }

        private static void CollectRows(Transform root, List<RectTransform> rows)
        {
            foreach (Transform child in root)
            {
                if (child is RectTransform rect && child.name.EndsWith("Row"))
                {
                    rows.Add(rect);
                }

                CollectRows(child, rows);
            }
        }

        private static RectTransform Find(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root as RectTransform;
            }

            foreach (Transform child in root)
            {
                RectTransform result = Find(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
