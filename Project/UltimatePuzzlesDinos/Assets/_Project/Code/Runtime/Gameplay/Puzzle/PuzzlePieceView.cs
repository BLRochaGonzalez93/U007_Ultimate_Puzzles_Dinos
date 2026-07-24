using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRMGames.UltimatePuzzlesDinos.Audio;
using VRMGames.UltimatePuzzlesDinos.Haptics;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle
{
    [DisallowMultipleComponent]
    public sealed class PuzzlePieceView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image image;
        [SerializeField] private Text label;

        private Canvas rootCanvas;
        private RectTransform homeSlot;
        private RectTransform targetSlot;
        private int pieceIndex;
        private bool placed;
        private Action<PuzzlePieceView> placedCallback;
        private Action<bool> moveCompletedCallback;

        public int PieceIndex => pieceIndex;
        public bool IsPlaced => placed;

        public void Initialize(
            int index,
            Canvas canvas,
            RectTransform initialHomeSlot,
            RectTransform target,
            Sprite pieceSprite,
            Color fallbackColor,
            Action<PuzzlePieceView> onPlaced,
            Action<bool> onMoveCompleted)
        {
            pieceIndex = index;
            rootCanvas = canvas;
            homeSlot = initialHomeSlot;
            targetSlot = target;
            placedCallback = onPlaced;
            moveCompletedCallback = onMoveCompleted;
            placed = false;

            rectTransform ??= GetComponent<RectTransform>();
            canvasGroup ??= GetComponent<CanvasGroup>();
            image ??= GetComponent<Image>();
            label ??= GetComponentInChildren<Text>(true);

            image.sprite = pieceSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.color = pieceSprite != null ? Color.white : fallbackColor;

            if (label != null)
            {
                label.gameObject.SetActive(pieceSprite == null);
            }

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            AttachTo(homeSlot);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (placed || rootCanvas == null)
            {
                return;
            }

            rectTransform.SetParent(rootCanvas.transform, true);
            rectTransform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.88f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (placed || rootCanvas == null)
            {
                return;
            }

            rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (placed)
            {
                return;
            }

            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            bool correctPlacement = targetSlot != null &&
                RectTransformUtility.RectangleContainsScreenPoint(
                    targetSlot,
                    eventData.position,
                    eventData.pressEventCamera);

            if (correctPlacement)
            {
                placed = true;
                AttachTo(targetSlot);
                canvasGroup.blocksRaycasts = false;

                AudioService.PlaySfx(AudioCue.PieceCorrect);
                HapticService.Play(HapticCue.Success);

                moveCompletedCallback?.Invoke(true);
                placedCallback?.Invoke(this);
                return;
            }

            AttachTo(homeSlot);

            AudioService.PlaySfx(AudioCue.PieceIncorrect);
            HapticService.Play(HapticCue.Error);

            moveCompletedCallback?.Invoke(false);
        }

        private void AttachTo(RectTransform parent)
        {
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(3f, 3f);
            rectTransform.offsetMax = new Vector2(-3f, -3f);
            rectTransform.localScale = Vector3.one;
        }
    }
}
