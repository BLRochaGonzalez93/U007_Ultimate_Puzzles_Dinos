using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Content;
using VRMGames.UltimatePuzzlesDinos.Gameplay;
using VRMGames.UltimatePuzzlesDinos.Monetization;
using VRMGames.UltimatePuzzlesDinos.Navigation;

namespace VRMGames.UltimatePuzzlesDinos.UI.Screens
{
    [DisallowMultipleComponent]
    public sealed class LevelSelectionScreen : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Text modeLabel;
        [SerializeField] private PuzzleCatalog puzzleCatalog;
        [SerializeField] private List<Button> levelButtons = new();
        [SerializeField] private List<Text> levelLabels = new();
        [SerializeField] private List<Image> previewImages = new();
        [SerializeField] private List<Text> previewPlaceholders = new();
        [SerializeField] private List<GameObject> lockIndicators = new();

        private readonly List<UnityEngine.Events.UnityAction> listeners = new();
        private GameObject rewardDialog;
        private Text rewardDialogTitle;
        private Text rewardDialogBody;
        private Button rewardConfirmButton;
        private Button rewardCancelButton;
        private int pendingRewardLevelId;

        private void Awake()
        {
            BuildRewardDialog();
        }

        private void OnEnable()
        {
            backButton?.onClick.AddListener(SceneNavigator.OpenMainMenu);
            rewardConfirmButton?.onClick.AddListener(ConfirmRewardUnlock);
            rewardCancelButton?.onClick.AddListener(HideRewardDialog);
            ProgressService.ProgressChanged += Refresh;
            HideRewardDialog();
            Refresh();
        }

        private void OnDisable()
        {
            backButton?.onClick.RemoveListener(SceneNavigator.OpenMainMenu);
            rewardConfirmButton?.onClick.RemoveListener(ConfirmRewardUnlock);
            rewardCancelButton?.onClick.RemoveListener(HideRewardDialog);
            ProgressService.ProgressChanged -= Refresh;
            RemoveLevelListeners();
        }

        private void Refresh()
        {
            if (modeLabel != null)
            {
                modeLabel.text = PuzzleSession.GetModeDisplayName();
            }

            RemoveLevelListeners();
            IReadOnlyList<PuzzleLevelInfo> levels = PuzzleLevelCatalog.GetLevels();
            int count = Mathf.Min(levels.Count, levelButtons.Count);

            for (int index = 0; index < count; index++)
            {
                PuzzleLevelInfo level = levels[index];
                Button button = levelButtons[index];
                PuzzleDefinition definition = puzzleCatalog != null
                    ? puzzleCatalog.GetByLevelNumber(level.Id)
                    : null;

                if (button == null)
                {
                    listeners.Add(null);
                    continue;
                }

                // Las tarjetas bloqueadas que permiten anuncio deben seguir
                // recibiendo pulsaciones para abrir el diálogo recompensado.
                button.enabled = true;
                button.interactable = level.Unlocked ||
                    ProgressService.CanUnlockWithReward(
                        PuzzleSession.SelectedMode,
                        level.Id);

                EnsureButtonRaycast(button);

                if (index < levelLabels.Count && levelLabels[index] != null)
                {
                    levelLabels[index].text = level.Stars > 0
                        ? $"{level.DisplayName}\n{BuildStarsText(level.Stars)}"
                        : level.DisplayName;
                }

                if (index < previewImages.Count && previewImages[index] != null)
                {
                    Image preview = previewImages[index];
                    preview.sprite = definition != null ? definition.Image : null;
                    preview.preserveAspect = true;
                    preview.color = preview.sprite != null ? Color.white : GetFallbackColor(level.Id, level.Unlocked);
                }

                if (index < previewPlaceholders.Count && previewPlaceholders[index] != null)
                {
                    Text placeholder = previewPlaceholders[index];
                    bool hasImage = definition != null && definition.Image != null;
                    placeholder.gameObject.SetActive(!hasImage);
                    placeholder.text = level.Unlocked ? $"DINOSAURIO\n{level.Id:00}" : "?";
                }

                if (index < lockIndicators.Count && lockIndicators[index] != null)
                {
                    lockIndicators[index].SetActive(!level.Unlocked);
                }

                int capturedLevelId = level.Id;
                bool capturedUnlocked = level.Unlocked;
                UnityEngine.Events.UnityAction listener = () => HandleLevelPressed(capturedLevelId, capturedUnlocked);
                listeners.Add(listener);
                button.onClick.AddListener(listener);
            }
        }

        private void HandleLevelPressed(int levelId, bool unlocked)
        {
            if (unlocked)
            {
                OpenLevel(levelId);
                return;
            }

            if (ProgressService.CanUnlockWithReward(PuzzleSession.SelectedMode, levelId))
            {
                ShowRewardDialog(levelId);
            }
        }

        private void ShowRewardDialog(int levelId)
        {
            pendingRewardLevelId = levelId;
            if (rewardDialogTitle != null)
            {
                rewardDialogTitle.text = $"DESBLOQUEAR NIVEL {levelId:00}";
            }

            if (rewardDialogBody != null)
            {
                rewardDialogBody.text = RewardedUnlockService.IsDevelopmentSimulation
                    ? "MODO DE DESARROLLO\nSimula ver un anuncio para desbloquear este nivel permanentemente."
                    : "Mira un anuncio corto para desbloquear este nivel permanentemente.";
            }

            rewardDialog?.SetActive(true);
        }

        private void HideRewardDialog()
        {
            pendingRewardLevelId = 0;
            rewardDialog?.SetActive(false);
        }

        private void ConfirmRewardUnlock()
        {
            int levelId = pendingRewardLevelId;
            if (levelId <= 0 || !RewardedUnlockService.IsAvailable)
            {
                return;
            }

            rewardConfirmButton.interactable = false;
            RewardedUnlockService.ShowLevelUnlockReward(
                PuzzleSession.SelectedMode,
                levelId,
                rewarded =>
                {
                    rewardConfirmButton.interactable = true;
                    if (!rewarded)
                    {
                        return;
                    }

                    ProgressService.UnlockLevelWithReward(PuzzleSession.SelectedMode, levelId);
                    HideRewardDialog();
                    OpenLevel(levelId);
                });
        }

        private static void EnsureButtonRaycast(Button button)
        {
            if (button == null)
            {
                return;
            }

            Image cardImage = button.GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.raycastTarget = true;
                button.targetGraphic = cardImage;
            }

            CanvasGroup[] groups =
                button.GetComponentsInParent<CanvasGroup>(true);

            foreach (CanvasGroup group in groups)
            {
                if (group == null)
                {
                    continue;
                }

                group.interactable = true;
                group.blocksRaycasts = true;
            }

            Transform lockIndicator = FindChild(button.transform, "LockIndicator");
            if (lockIndicator != null)
            {
                Graphic[] graphics =
                    lockIndicator.GetComponentsInChildren<Graphic>(true);

                foreach (Graphic graphic in graphics)
                {
                    if (graphic != null)
                    {
                        graphic.raycastTarget = false;
                    }
                }
            }
        }

        private static Transform FindChild(Transform root, string targetName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == targetName)
            {
                return root;
            }

            foreach (Transform child in root)
            {
                Transform result = FindChild(child, targetName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static string BuildStarsText(int stars)
        {
            int normalizedStars = Mathf.Clamp(stars, 0, 4);
            return new string('★', normalizedStars) + new string('☆', 4 - normalizedStars);
        }

        private static Color GetFallbackColor(int levelId, bool unlocked)
        {
            if (!unlocked)
            {
                return new Color(0.11f, 0.11f, 0.11f, 1f);
            }

            float hue = Mathf.Repeat(0.04f + (levelId - 1) * 0.067f, 1f);
            return Color.HSVToRGB(hue, 0.52f, 0.56f);
        }

        private static void OpenLevel(int levelId)
        {
            PuzzleSession.SelectLevel(levelId);
            SceneNavigator.OpenDifficultySelection();
        }

        private void RemoveLevelListeners()
        {
            int count = Mathf.Min(levelButtons.Count, listeners.Count);
            for (int index = 0; index < count; index++)
            {
                if (levelButtons[index] != null && listeners[index] != null)
                {
                    levelButtons[index].onClick.RemoveListener(listeners[index]);
                }
            }

            listeners.Clear();
        }

        private void BuildRewardDialog()
        {
            if (rewardDialog != null)
            {
                return;
            }

            Canvas canvas = GetComponentInParent<Canvas>();
            Transform parent = canvas != null ? canvas.transform : transform;
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            rewardDialog = CreateUiObject("RewardUnlockDialog", parent, typeof(Image));
            RectTransform dialogRect = rewardDialog.GetComponent<RectTransform>();
            Stretch(dialogRect);
            rewardDialog.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.78f);

            GameObject card = CreateUiObject("Card", rewardDialog.transform, typeof(Image), typeof(VerticalLayoutGroup));
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0.5f);
            cardRect.anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.pivot = new Vector2(0.5f, 0.5f);
            cardRect.sizeDelta = new Vector2(760f, 430f);
            cardRect.anchoredPosition = Vector2.zero;
            card.GetComponent<Image>().color = new Color(0.16f, 0.11f, 0.07f, 0.98f);
            VerticalLayoutGroup layout = card.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(42, 42, 34, 34);
            layout.spacing = 22f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            rewardDialogTitle = CreateText("Title", card.transform, font, 42, FontStyle.Bold, TextAnchor.MiddleCenter);
            rewardDialogTitle.gameObject.AddComponent<LayoutElement>().preferredHeight = 70f;
            rewardDialogTitle.color = new Color(1f, 0.82f, 0.2f, 1f);

            rewardDialogBody = CreateText("Body", card.transform, font, 27, FontStyle.Normal, TextAnchor.MiddleCenter);
            rewardDialogBody.gameObject.AddComponent<LayoutElement>().preferredHeight = 120f;
            rewardDialogBody.color = Color.white;

            GameObject buttons = CreateUiObject("Buttons", card.transform, typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            buttons.GetComponent<LayoutElement>().preferredHeight = 86f;
            HorizontalLayoutGroup buttonLayout = buttons.GetComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 24f;
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = true;
            buttonLayout.childForceExpandWidth = true;
            buttonLayout.childForceExpandHeight = true;

            rewardCancelButton = CreateButton("CancelButton", buttons.transform, "CANCELAR", font, new Color(0.36f, 0.18f, 0.12f, 1f));
            rewardConfirmButton = CreateButton("RewardButton", buttons.transform, "VER ANUNCIO", font, new Color(0.22f, 0.56f, 0.12f, 1f));
            rewardDialog.SetActive(false);
        }

        private static GameObject CreateUiObject(string name, Transform parent, params System.Type[] components)
        {
            GameObject result = new(name, typeof(RectTransform));
            result.transform.SetParent(parent, false);
            foreach (System.Type component in components)
            {
                result.AddComponent(component);
            }

            return result;
        }

        private static Text CreateText(string name, Transform parent, Font font, int size, FontStyle style, TextAnchor alignment)
        {
            GameObject gameObject = CreateUiObject(name, parent, typeof(Text));
            Text text = gameObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Font font, Color color)
        {
            GameObject gameObject = CreateUiObject(name, parent, typeof(Image), typeof(Button));
            Image image = gameObject.GetComponent<Image>();
            image.color = color;
            Button button = gameObject.GetComponent<Button>();
            button.targetGraphic = image;
            Text text = CreateText("Label", gameObject.transform, font, 28, FontStyle.Bold, TextAnchor.MiddleCenter);
            text.text = label;
            text.color = Color.white;
            Stretch(text.rectTransform);
            return button;
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
