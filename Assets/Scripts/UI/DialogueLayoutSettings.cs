using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    /// <summary>
    /// 特定のオブジェクトとインタラクトする際に適用される、ダイアログパネルのレイアウト設定を保持するコンポーネント。
    /// </summary>
    public class DialogueLayoutSettings : MonoBehaviour
    {
        [Header("RectTransform Settings")]
        public Vector2 anchorMin = new Vector2(0, 0);      // アンカー（最小）
        public Vector2 anchorMax = new Vector2(1, 0.25f); // アンカー（最大）
        public Vector2 pivot = new Vector2(0.5f, 0);       // ピボット
        public Vector2 anchoredPosition = Vector2.zero;    // 座標
        public Vector2 sizeDelta = Vector2.zero;           // サイズ（サイズデルタ）

        /// <summary>
        /// これらの設定をDialogueManagerに一時的なレイアウトとして適用する。
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
