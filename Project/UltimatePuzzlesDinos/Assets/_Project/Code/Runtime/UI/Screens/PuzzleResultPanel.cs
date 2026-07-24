using System;
using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class PuzzleResultPanel : MonoBehaviour
    {
        private GameObject overlay;
        private Text titleLabel;
        private Text levelLabel;
        private Text earnedStarsLabel;
        private Text bestStarsLabel;
        private Text resultMessageLabel;
        private Text statsLabel;
        private Button replayButton;
        private Button levelsButton;
        private Text replayButtonLabel;
        private Text levelsButtonLabel;
        private Action replayAction;
        private Action levelsAction;

        public void Initialize(Action onReplay, Action onLevels)
        {
            replayAction = onReplay;
            levelsAction = onLevels;
            EnsureBuilt();
            ApplyButtonLabels();

            replayButton.onClick.RemoveAllListeners();
            replayButton.onClick.AddListener(() => replayAction?.Invoke());
            levelsButton.onClick.RemoveAllListeners();
            levelsButton.onClick.AddListener(() => levelsAction?.Invoke());
        }

        public void Show(PuzzleCompletionResult result)
        {
            EnsureBuilt();
            ApplyButtonLabels();

            titleLabel.text = "¡PUZZLE COMPLETADO!";
            levelLabel.text = $"NIVEL {result.LevelId:00} · {PuzzleDifficultyCatalog.Get(result.Difficulty).DisplayName}";
            earnedStarsLabel.text = BuildStars(result.EarnedStars);
            bestStarsLabel.text = $"MEJOR RESULTADO  {BuildStars(result.BestStars)}";
            resultMessageLabel.text = result.ImprovedBest
                ? result.PreviousBestStars > 0 ? "¡NUEVA MEJOR MARCA!" : "¡PRIMERA VICTORIA!"
                : "RESULTADO GUARDADO";
            statsLabel.text = $"TIEMPO  {FormatTime(result.ElapsedSeconds)}   ·   MOVIMIENTOS  {result.Moves}";
            overlay.SetActive(true);
        }

        public void Hide()
        {
            if (overlay != null)
            {
                overlay.SetActive(false);
            }
        }

        private void EnsureBuilt()
        {
            if (overlay != null)
            {
                return;
            }

            Canvas canvas = GetComponentInParent<Canvas>();
            Transform parent = canvas != null ? canvas.transform : transform;
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            overlay = CreateObject("PuzzleResultOverlay", parent, typeof(Image));
            Stretch(overlay.GetComponent<RectTransform>());
            overlay.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

            GameObject card = CreateObject("ResultCard", overlay.transform, typeof(Image), typeof(VerticalLayoutGroup));
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(900f, 680f);
            cardRect.anchoredPosition = Vector2.zero;
            card.GetComponent<Image>().color = new Color(0.14f, 0.09f, 0.05f, 0.98f);

            VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(56, 56, 38, 38);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            titleLabel = CreateText("Title", card.transform, font, 48, FontStyle.Bold, new Color(1f, 0.78f, 0.12f, 1f));
            SetPreferredHeight(titleLabel.gameObject, 72f);

            levelLabel = CreateText("Level", card.transform, font, 28, FontStyle.Bold, Color.white);
            SetPreferredHeight(levelLabel.gameObject, 44f);

            earnedStarsLabel = CreateText("EarnedStars", card.transform, font, 72, FontStyle.Bold, new Color(1f, 0.79f, 0.08f, 1f));
            SetPreferredHeight(earnedStarsLabel.gameObject, 98f);

            resultMessageLabel = CreateText("Message", card.transform, font, 30, FontStyle.Bold, new Color(0.48f, 0.92f, 0.28f, 1f));
            SetPreferredHeight(resultMessageLabel.gameObject, 48f);

            bestStarsLabel = CreateText("BestStars", card.transform, font, 26, FontStyle.Normal, new Color(0.93f, 0.88f, 0.75f, 1f));
            SetPreferredHeight(bestStarsLabel.gameObject, 44f);

            statsLabel = CreateText("Stats", card.transform, font, 24, FontStyle.Bold, new Color(0.82f, 0.78f, 0.68f, 1f));
            SetPreferredHeight(statsLabel.gameObject, 44f);

            GameObject buttons = CreateObject("Buttons", card.transform, typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            buttons.GetComponent<LayoutElement>().preferredHeight = 96f;
            HorizontalLayoutGroup buttonLayout = buttons.GetComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 24f;
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = true;
            buttonLayout.childForceExpandWidth = true;
            buttonLayout.childForceExpandHeight = true;

            replayButton = CreateButton("ReplayButton", buttons.transform, font, new Color(0.58f, 0.23f, 0.10f, 1f), out replayButtonLabel);
            levelsButton = CreateButton("LevelsButton", buttons.transform, font, new Color(0.20f, 0.53f, 0.12f, 1f), out levelsButtonLabel);
            ApplyButtonLabels();
            overlay.SetActive(false);
        }

        private void ApplyButtonLabels()
        {
            if (replayButtonLabel != null)
            {
                replayButtonLabel.text = "REPETIR PUZZLE";
                replayButtonLabel.gameObject.SetActive(true);
            }

            if (levelsButtonLabel != null)
            {
                levelsButtonLabel.text = "VOLVER A NIVELES";
                levelsButtonLabel.gameObject.SetActive(true);
            }
        }

        private static string BuildStars(int stars)
        {
            int value = Mathf.Clamp(stars, 0, 4);
            return new string('★', value) + new string('☆', 4 - value);
        }

        private static string FormatTime(float seconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.RoundToInt(seconds));
            int minutes = totalSeconds / 60;
            int remainingSeconds = totalSeconds % 60;
            return $"{minutes:00}:{remainingSeconds:00}";
        }

        private static GameObject CreateObject(string name, Transform parent, params Type[] components)
        {
            GameObject result = new(name, typeof(RectTransform));
            result.transform.SetParent(parent, false);
            foreach (Type component in components)
            {
                result.AddComponent(component);
            }

            return result;
        }

        private static Text CreateText(string name, Transform parent, Font font, int size, FontStyle style, Color color)
        {
            GameObject result = CreateObject(name, parent, typeof(Text));
            Text text = result.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, Font font, Color color, out Text label)
        {
            GameObject result = CreateObject(name, parent, typeof(Image), typeof(Button), typeof(LayoutElement));
            LayoutElement layoutElement = result.GetComponent<LayoutElement>();
            layoutElement.minWidth = 300f;
            layoutElement.preferredWidth = 360f;
            layoutElement.preferredHeight = 88f;

            Image image = result.GetComponent<Image>();
            image.color = color;
            Button button = result.GetComponent<Button>();
            button.targetGraphic = image;

            label = CreateText("Label", result.transform, font, 26, FontStyle.Bold, Color.white);
            RectTransform labelRect = label.rectTransform;
            Stretch(labelRect);
            labelRect.offsetMin = new Vector2(12f, 8f);
            labelRect.offsetMax = new Vector2(-12f, -8f);
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 18;
            label.resizeTextMaxSize = 28;
            label.transform.SetAsLastSibling();
            return button;
        }

        private static void SetPreferredHeight(GameObject target, float height)
        {
            LayoutElement element = target.AddComponent<LayoutElement>();
            element.preferredHeight = height;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
