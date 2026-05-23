using UnityEngine;

namespace ShiraOzi.Interaction
{
    /// <summary>
    /// マウスカーソルの形状を管理するマネージャークラス。
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; } // シングルトンインスタンス

        [SerializeField] private Texture2D defaultCursor;  // デフォルトのカーソルテクスチャ
        [SerializeField] private Texture2D interactCursor; // インタラクト時のカーソルテクスチャ
        [SerializeField] private Vector2 hotspot = Vector2.zero; // カーソルのクリック判定位置

        private void Awake()
        {
            // シングルトンの初期化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// カーソルをデフォルトの形状に設定する。
        /// </summary>
        public void SetDefaultCursor()
        {
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }

        /// <summary>
        /// カーソルをインタラクト用の形状に設定する。
        /// </summary>
        public void SetInteractCursor()
        {
            Cursor.SetCursor(interactCursor, hotspot, CursorMode.Auto);
        }
    }
}
