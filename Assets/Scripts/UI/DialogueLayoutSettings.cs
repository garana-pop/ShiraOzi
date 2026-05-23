using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    /// <summary>
    /// Holds layout settings for the DialoguePanel to be applied when interacting with specific objects.
    /// </summary>
    public class DialogueLayoutSettings : MonoBehaviour
    {
        [Header("RectTransform Settings")]
        public Vector2 anchorMin = new Vector2(0, 0);
        public Vector2 anchorMax = new Vector2(1, 0.25f);
        public Vector2 pivot = new Vector2(0.5f, 0);
        public Vector2 anchoredPosition = Vector2.zero;
        public Vector2 sizeDelta = Vector2.zero;

        /// <summary>
        /// Applies these settings to the DialogueManager.
        /// </summary>
        public void Apply()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.SetTemporaryLayout(this);
            }
        }
    }
}
