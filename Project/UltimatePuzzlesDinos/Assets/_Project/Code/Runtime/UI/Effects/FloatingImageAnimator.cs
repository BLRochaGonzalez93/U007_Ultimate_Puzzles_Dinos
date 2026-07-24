using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.UI.Effects
{
    [DisallowMultipleComponent]
    public sealed class FloatingImageAnimator : MonoBehaviour
    {
        [SerializeField] private Vector2 movementAmplitude = new(20f, 12f);
        [SerializeField] private Vector2 movementFrequency = new(0.55f, 0.4f);
        [SerializeField] private float rotationAmplitude = 8f;
        [SerializeField] private float rotationFrequency = 0.35f;
        [SerializeField] private float phaseOffset;

        private RectTransform rectTransform;
        private Vector2 initialAnchoredPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                initialAnchoredPosition = rectTransform.anchoredPosition;
            }

            initialRotation = transform.localRotation;
        }

        private void Update()
        {
            float t = Time.unscaledTime + phaseOffset;

            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = initialAnchoredPosition + new Vector2(
                    Mathf.Sin(t * movementFrequency.x) * movementAmplitude.x,
                    Mathf.Cos(t * movementFrequency.y) * movementAmplitude.y);
            }

            float z = Mathf.Sin(t * rotationFrequency) * rotationAmplitude;
            transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, z);
        }
    }
}
