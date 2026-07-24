using UnityEngine;
using UnityEngine.UI;

namespace VRMGames.UltimatePuzzlesDinos.UI.Responsive
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class ResponsiveCanvasController : MonoBehaviour
    {
        [SerializeField] private Vector2 portraitReferenceResolution = new(1080f, 1920f);
        [SerializeField] private Vector2 landscapeReferenceResolution = new(1920f, 1080f);
        [SerializeField, Range(0f, 1f)] private float portraitMatch = 0.5f;
        [SerializeField, Range(0f, 1f)] private float landscapeMatch = 0.5f;

        private CanvasScaler scaler;
        private int lastWidth;
        private int lastHeight;

        private void Awake()
        {
            scaler = GetComponent<CanvasScaler>();
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                Apply();
            }
        }

        public void Apply()
        {
            if (scaler == null)
            {
                scaler = GetComponent<CanvasScaler>();
            }

            bool portrait = Screen.height >= Screen.width;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = portrait
                ? portraitReferenceResolution
                : landscapeReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = portrait ? portraitMatch : landscapeMatch;
            scaler.referencePixelsPerUnit = 100f;

            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }
}
