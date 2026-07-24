using UnityEngine;

namespace VRMGames.UltimatePuzzlesDinos.UI.Effects
{
    [DisallowMultipleComponent]
    public sealed class ContinuousRotateUI : MonoBehaviour
    {
        [SerializeField] private float degreesPerSecond = 7f;

        private void Update()
        {
            transform.Rotate(0f, 0f, degreesPerSecond * Time.unscaledDeltaTime);
        }
    }
}
