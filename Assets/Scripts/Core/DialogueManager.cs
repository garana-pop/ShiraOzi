using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using ShiraOzi.UI;
using System.Collections;

namespace ShiraOzi.Core
{
    /// <summary>
    /// ゲーム内の会話（ダイアログ）の進行を制御するマネージャークラス。
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; } // シングルトンインスタンス

        [SerializeField] private GameState gameState; // ゲーム状態データへの参照
        [SerializeField] private SpeakerMapping speakerMapping; // textKey サフィックスと話者名の対応表
        
        public bool IsDisplaying => isDisplaying;
        private bool isDisplaying; // 現在会話を表示中かどうかのフラグ
        private DialogueEntry currentEntry; // 現在再生中のダイアログエントリ
        private int currentLineIndex; // 現在表示中の行のインデックス
        private DialogueLayoutSettings tempLayout; // 一時的に適用されるレイアウト設定

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

        private void OnEnable()
        {
            // 言語設定が変更されたときのイベントを購読
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        private void OnDisable()
        {
            // イベント購読の解除
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        /// <summary>
        /// 言語が変更されたときに呼び出される。
        /// </summary>
        /// <param name="locale">新しいロケール</param>
        private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
        {
            // 会話表示中の場合はテキストを再描画
            if (isDisplaying)
            {
                DisplayCurrentLine();
            }
        }

        /// <summary>
        /// 指定されたダイアログエントリを使用して会話を開始する。
        /// </summary>
        /// <param name="entry">開始するダイアログエントリ</param>
        public void StartDialogue(DialogueEntry entry)
        {
            if (entry == null || isDisplaying) return;
            
            currentEntry = entry;
            currentLineIndex = 0;
            isDisplaying = true;
            
            // 特殊なレイアウト設定がある場合は適用
            if (tempLayout != null && UIManager.Instance != null)
            {
                UIManager.Instance.SetDialogueLayout(tempLayout);
            }

            DisplayCurrentLine();
        }

        /// <summary>
        /// 次に開始する会話のための一時的なレイアウトを設定する。
        /// </summary>
        /// <param name="layout">レイアウト設定</param>
        public void SetTemporaryLayout(DialogueLayoutSettings layout)
        {
            tempLayout = layout;
        }

        /// <summary>
        /// 会話を1行進める。最後の行の場合は会話を終了する。
        /// </summary>
        public void AdvanceDialogue()
        {
            if (!isDisplaying) return;

            currentLineIndex++;
            if (currentLineIndex < currentEntry.lines.Length)
            {
                DisplayCurrentLine(); // 次の行を表示
            }
            else
            {
                EndDialogue(); // 会話を終了
            }
        }

        /// <summary>
        /// 現在のインデックスに対応する会話行を表示する。
        /// </summary>
        private void DisplayCurrentLine()
        {
            var line = currentEntry.lines[currentLineIndex];

            // セリフ本文はローカライズキー（textKey）から取得
            string text = GetLocalizedString("UIStrings", line.textKey);

            string speaker;
            bool showSpeaker = true;

            // textKey のサフィックス（例: _Ozi, _Protagonist, _Narration）から話者を決定。
            // マッピングに存在しない場合は従来の speakerKey にフォールバックする。
            string suffix = ExtractSuffix(line.textKey);
            if (speakerMapping != null && speakerMapping.TryGet(suffix, out var entry))
            {
                if (entry.hideSpeaker)
                {
                    // ナレーション等：話者ラベルを非表示にする
                    speaker = string.Empty;
                    showSpeaker = false;
                }
                else
                {
                    speaker = GetLocalizedString("UIStrings", entry.speakerNameKey);
                }
            }
            else
            {
                // フォールバック：従来の speakerKey を使用
                speaker = GetLocalizedString("UIStrings", line.speakerKey);
            }

            if (UIManager.Instance)
            {
                UIManager.Instance.ShowDialogue(speaker, text, showSpeaker);
            }
        }

        /// <summary>
        /// ローカライズキーの最後の '_' 以降をサフィックスとして抽出する。
        /// </summary>
        /// <param name="key">対象のローカライズキー（textKey）</param>
        /// <returns>サフィックス。'_' が無い／末尾が '_' の場合は null。</returns>
        private static string ExtractSuffix(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            int idx = key.LastIndexOf('_');
            return idx >= 0 && idx < key.Length - 1 ? key.Substring(idx + 1) : null;
        }

        /// <summary>
        /// 会話を終了し、UIを閉じる。
        /// </summary>
        public void EndDialogue()
        {
isDisplaying = false;
            if (UIManager.Instance)
            {
                UIManager.Instance.HideDialogue();
                UIManager.Instance.ResetDialogueLayout();
            }
            tempLayout = null; // レイアウト設定をクリア
        }

        /// <summary>
        /// ローカライズテーブルからキーに対応する文字列を取得する。
        /// </summary>
        /// <param name="tableReference">テーブル名</param>
        /// <param name="key">エントリキー</param>
        /// <returns>ローカライズされた文字列</returns>
        private string GetLocalizedString(string tableReference, string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableReference, key);
        }
    }
}
