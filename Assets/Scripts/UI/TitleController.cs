using UnityEngine;
using UnityEngine.SceneManagement;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    /// <summary>
    /// タイトルシーンのボタンイベント（開始、設定、終了）を制御するクラス。
    /// </summary>
    public class TitleController : MonoBehaviour
    {
        [Header("Data")]
        public GameState gameState;

        [Header("Scenes")]
        public string openingSceneName = "OpeningScene";
        public string mainSceneName = "MainScene";

        [Header("UI Panels")]
        public GameObject settingsPanel;

        /// <summary>
        /// スタートボタン押下時の処理。
        /// 進行状況に応じてオープニングかメインシーンに遷移する。
        /// </summary>
        public void OnStartClicked()
        {
            if (gameState == null)
            {
                Debug.LogError("GameState is not assigned to TitleController.");
                return;
            }

            if (!gameState.hasSeenOpening)
            {
                // オープニングをまだ見ていない場合はフラグを立ててオープニングシーンへ
                gameState.hasSeenOpening = true;
                SceneManager.LoadScene(openingSceneName);
            }
            else
            {
                // すでにオープニングを見たことがある場合はメインシーンへ
                SceneManager.LoadScene(mainSceneName);
            }
        }

        /// <summary>
        /// 設定ボタン押下時の処理。
        /// 設定パネルの表示状態を切り替える。
        /// </summary>
        public void OnSettingsClicked()
        {
            if (settingsPanel)
            {
                settingsPanel.SetActive(!settingsPanel.activeSelf);
            }
        }

        /// <summary>
        /// ゲーム終了ボタン押下時の処理。
        /// アプリケーションを終了する。
        /// </summary>
        public void OnGameEndClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
