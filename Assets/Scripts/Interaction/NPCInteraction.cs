using UnityEngine;
using ShiraOzi.Core;
using ShiraOzi.UI;

namespace ShiraOzi.Interaction
{
    /// <summary>
    /// NPCとのインタラクションを管理するコンポーネント。
    /// プレイヤーが持っているアイテムに応じて異なる会話を再生する。
    /// </summary>
    public class NPCInteraction : MonoBehaviour
    {
        [SerializeField] private GameState gameState; // ゲーム状態データへの参照
        [SerializeField] private DialogueEntry defaultDialogue; // デフォルトの会話データ
        [SerializeField] private DialogueEntry[] itemSpecificDialogues; // 特定のアイテム所持時に再生する会話データ配列
        [SerializeField] private string[] itemIDs; // 対応するアイテムIDの配列

        /// <summary>
        /// NPCと対話を開始する。
        /// </summary>
        public void Interact()
        {
            // このオブジェクトに独自のレイアウト設定があれば適用
            if (TryGetComponent<DialogueLayoutSettings>(out var layoutSettings))
            {
                layoutSettings.Apply();
            }

            DialogueEntry toPlay = defaultDialogue;

            // プレイヤーがアイテムを持っている場合、対応する会話があるかチェック
            if (gameState != null && !string.IsNullOrEmpty(gameState.heldItemID))
            {
                for (int i = 0; i < itemIDs.Length; i++)
                {
                    if (gameState.heldItemID == itemIDs[i])
                    {
                        if (i < itemSpecificDialogues.Length)
                        {
                            toPlay = itemSpecificDialogues[i];
                        }
                        break;
                    }
                }
            }

            // 決定された会話を再生
            if (DialogueManager.Instance)
            {
                DialogueManager.Instance.StartDialogue(toPlay);
            }
        }
    }
}
