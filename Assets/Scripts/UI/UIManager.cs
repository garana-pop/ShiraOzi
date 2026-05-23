using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    /// <summary>
    /// ゲーム全体のUI表示を管理するマネージャークラス。
    /// ダイアログ、インベントリ、設定画面などの表示切り替えを行う。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; } // シングルトンインスタンス

        [Header("Dialogue UI")]
        public GameObject dialoguePanel; // 会話パネル
        public TextMeshProUGUI speakerText; // 話者名テキスト
        public TextMeshProUGUI dialogueText; // セリフテキスト
        public UnityEngine.UI.Button advanceButton; // 会話を進めるボタン

        [Header("Inventory UI")]
        public GameObject itemPanel; // アクティブアイテム表示パネル
        public Image activeItemImage; // アクティブアイテムのアイコン
        public TextMeshProUGUI itemText; // アクティブアイテム名
        public InventoryUI inventoryUI; // インベントリUIの参照

        [Header("Global Data")]
        public GameState gameState; // ゲーム状態データへの参照

        [Header("Settings UI")]
        public GameObject settingsPanel; // 設定画面パネル

        private RectTransform dialogueRectTransform; // 会話パネルのRectTransform
        private Vector2 defaultAnchorMin; // デフォルトのアンカー（最小）
        private Vector2 defaultAnchorMax; // デフォルトのアンカー（最大）
        private Vector2 defaultPivot; // デフォルトのピボット
        private Vector2 defaultAnchoredPosition; // デフォルトの座標
        private Vector2 defaultSizeDelta; // デフォルトのサイズ

        private void Awake()
        {
            // シングルトンの初期化とデフォルトレイアウトの保存
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (dialoguePanel)
                {
                    dialogueRectTransform = dialoguePanel.GetComponent<RectTransform>();
                    if (dialogueRectTransform)
                    {
                        defaultAnchorMin = dialogueRectTransform.anchorMin;
                        defaultAnchorMax = dialogueRectTransform.anchorMax;
                        defaultPivot = dialogueRectTransform.pivot;
                        defaultAnchoredPosition = dialogueRectTransform.anchoredPosition;
                        defaultSizeDelta = dialogueRectTransform.sizeDelta;
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // ボタンイベントの登録
            if (advanceButton)
            {
                advanceButton.onClick.AddListener(OnAdvanceClicked);
            }

            if (itemPanel)
            {
                UnityEngine.UI.Button itemBtn = itemPanel.GetComponent<UnityEngine.UI.Button>();
                if (itemBtn) itemBtn.onClick.AddListener(OnItemPanelClicked);
            }

            // 初期表示の更新
            RefreshItemDisplay();
        }

        private void OnEnable()
        {
            // アイテム変更イベントを購読
            if (gameState) gameState.OnItemChanged += RefreshItemDisplay;
        }

        private void OnDisable()
        {
            // イベント購読の解除
            if (gameState) gameState.OnItemChanged -= RefreshItemDisplay;
        }

        /// <summary>
        /// ダイアログを表示し、テキストを設定する。
        /// </summary>
        /// <param name="speaker">話者の名前</param>
        /// <param name="text">表示するセリフ</param>
        public void ShowDialogue(string speaker, string text)
        {
            if (dialoguePanel) dialoguePanel.SetActive(true);
            if (speakerText) speakerText.text = speaker;
            if (dialogueText) dialogueText.text = text;
        }

        /// <summary>
        /// ダイアログパネルのレイアウトを一時的に変更する。
        /// </summary>
        /// <param name="settings">適用するレイアウト設定</param>
        public void SetDialogueLayout(DialogueLayoutSettings settings)
        {
            if (dialogueRectTransform == null || settings == null) return;

            dialogueRectTransform.anchorMin = settings.anchorMin;
            dialogueRectTransform.anchorMax = settings.anchorMax;
            dialogueRectTransform.pivot = settings.pivot;
            dialogueRectTransform.anchoredPosition = settings.anchoredPosition;
            dialogueRectTransform.sizeDelta = settings.sizeDelta;
        }

        /// <summary>
        /// ダイアログパネルのレイアウトを初期状態に戻す。
        /// </summary>
        public void ResetDialogueLayout()
        {
            if (dialogueRectTransform == null) return;

            dialogueRectTransform.anchorMin = defaultAnchorMin;
            dialogueRectTransform.anchorMax = defaultAnchorMax;
            dialogueRectTransform.pivot = defaultPivot;
            dialogueRectTransform.anchoredPosition = defaultAnchoredPosition;
            dialogueRectTransform.sizeDelta = defaultSizeDelta;
        }

        /// <summary>
        /// ダイアログパネルを非表示にする。
        /// </summary>
        public void HideDialogue()
        {
            if (dialoguePanel) dialoguePanel.SetActive(false);
        }

        /// <summary>
        /// 現在のアクティブアイテムに基づいてUI表示を更新する。
        /// </summary>
        public void RefreshItemDisplay()
        {
            if (gameState == null) return;

            var item = gameState.activeItem;
            bool hasItem = item != null;

            // アイテムがある場合はパネルを表示
            if (itemPanel && hasItem && !itemPanel.activeSelf)
            {
                itemPanel.SetActive(true);
            }

            if (activeItemImage) activeItemImage.sprite = hasItem ? item.icon : null;
            if (itemText) itemText.text = hasItem ? (item.itemName ?? string.Empty) : string.Empty;
        }

        /// <summary>
        /// アイテムパネルがクリックされたときに呼び出される。
        /// </summary>
        public void OnItemPanelClicked()
        {
            // 会話中の場合は会話を終了する
            if (DialogueManager.Instance && DialogueManager.Instance.IsDisplaying)
            {
                DialogueManager.Instance.EndDialogue();
            }

            if (inventoryUI) inventoryUI.Toggle();
        }

        /// <summary>
        /// 設定パネルの表示/非表示を切り替える。
        /// </summary>
        public void ToggleSettings()
        {
            if (settingsPanel) settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        /// <summary>
        /// 会話進行ボタンがクリックされたときに呼び出される。
        /// </summary>
        private void OnAdvanceClicked()
        {
            if (DialogueManager.Instance)
            {
                DialogueManager.Instance.AdvanceDialogue();
            }
        }
    }
}