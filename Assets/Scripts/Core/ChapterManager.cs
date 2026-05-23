using UnityEngine;

namespace ShiraOzi.Core
{
    /// <summary>
    /// ゲームのチャプター進行を管理するマネージャークラス。
    /// </summary>
    public class ChapterManager : MonoBehaviour
    {
        public static ChapterManager Instance { get; private set; } // シングルトンインスタンス

        public GameState gameState; // ゲーム状態データへの参照

        private void Awake()
        {
            // シングルトンの初期化処理
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
        /// 次のチャプターへ進める。
        /// </summary>
        public void NextChapter()
        {
            if (gameState)
            {
                // チャプター番号をインクリメント
                gameState.currentChapter++;
                Debug.Log($"Moved to Chapter: {gameState.currentChapter}");
            }
        }

        /// <summary>
        /// 指定したチャプター番号に設定する。
        /// </summary>
        /// <param name="chapter">設定するチャプター番号</param>
        public void SetChapter(int chapter)
        {
            if (gameState)
            {
                gameState.currentChapter = chapter;
                Debug.Log($"Set Chapter: {gameState.currentChapter}");
            }
        }
    }
}
