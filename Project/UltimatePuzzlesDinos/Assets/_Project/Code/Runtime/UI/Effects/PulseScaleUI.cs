using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.UI.Effects
{
    [DisallowMultipleComponent]
    public sealed class PulseScaleUI : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.045f;
        [SerializeField] private float frequency = 1.8f;
        [SerializeField] private float phaseOffset;

        private Vector3 baseScale;

        private void Awake()
        {
            baseScale = transform.localScale;
        }

        private void Update()
        {
            float t = Time.unscaledTime + phaseOffset;
            float scale = 1f + Mathf.Sin(t * frequency) * amplitude;
            transform.localScale = baseScale * scale;
        }
    }
}
