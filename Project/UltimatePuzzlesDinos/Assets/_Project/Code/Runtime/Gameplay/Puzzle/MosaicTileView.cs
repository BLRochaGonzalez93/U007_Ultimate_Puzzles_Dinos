using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.Gameplay.Puzzle
{
    [DisallowMultipleComponent]
    public sealed class MosaicTileView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private Text label;

        private int tileIndex;
        private Action<MosaicTileView> clickedCallback;

        public int TileIndex => tileIndex;

        public void Initialize(
            int index,
            Sprite sprite,
            Color fallbackColor,
            string fallbackLabel,
            Action<MosaicTileView> onClicked)
        {
            tileIndex = index;
            clickedCallback = onClicked;

            image ??= GetComponent<Image>();
            label ??= GetComponentInChildren<Text>(true);

            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.color = sprite != null ? Color.white : fallbackColor;

            if (label != null)
            {
                label.gameObject.SetActive(sprite == null);
                label.text = fallbackLabel;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            clickedCallback?.Invoke(this);
        }
    }
}
