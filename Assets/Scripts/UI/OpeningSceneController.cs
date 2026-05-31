using UnityEngine;
using UnityEngine.SceneManagement;
using ShiraOzi.Core;
using System.Collections;

namespace ShiraOzi.UI
{
    /// <summary>
    /// オープニングシーンの進行（会話開始からシーン遷移まで）を管理するクラス。
    /// </summary>
    public class OpeningSceneController : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private DialogueEntry openingDialogue; // 再生するダイアログデータ

        [Header("Next Scene")]
        [SerializeField] private string nextSceneName = "MainScene"; // 会話終了後に遷移するシーン名

        private bool dialogueStarted = false;

        private IEnumerator Start()
        {
            // Managers（特にDialogueManager）が初期化されるのを待つ
            yield return new WaitUntil(() => DialogueManager.Instance != null);

            StartOpeningDialogue();
        }

        /// <summary>
        /// オープニングの会話を開始する。
        /// </summary>
        public void StartOpeningDialogue()
        {
            if (openingDialogue != null && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(openingDialogue);
                dialogueStarted = true;
            }
            else
            {
                Debug.LogWarning("[OpeningSceneController] Dialogue data or DialogueManager is missing.");
                // 会話が開始できない場合は即座に遷移
                LoadNextScene();
            }
        }

        private void Update()
        {
            // 会話が開始されており、かつDialogueManagerが表示を終了した場合は次のシーンへ
            if (dialogueStarted && DialogueManager.Instance != null && !DialogueManager.Instance.IsDisplaying)
            {
                dialogueStarted = false;
                LoadNextScene();
            }
        }

        /// <summary>
        /// 次のシーンをロードする。
        /// </summary>
        private void LoadNextScene()
        {
            Debug.Log("[OpeningSceneController] Dialogue finished. Loading " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
