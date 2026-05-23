using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Dialogue UI")]
        public GameObject dialoguePanel;
        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI dialogueText;
        public UnityEngine.UI.Button advanceButton;

        [Header("Inventory UI")]
        public GameObject itemPanel;
        public Image activeItemImage;
        public TextMeshProUGUI itemText;
        public InventoryUI inventoryUI;

        [Header("Global Data")]
        public GameState gameState;

        [Header("Settings UI")]
        public GameObject settingsPanel;

        private RectTransform dialogueRectTransform;
        private Vector2 defaultAnchorMin;
        private Vector2 defaultAnchorMax;
        private Vector2 defaultPivot;
        private Vector2 defaultAnchoredPosition;
        private Vector2 defaultSizeDelta;

        private void Awake()
        {
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
            if (advanceButton)
            {
                advanceButton.onClick.AddListener(OnAdvanceClicked);
            }

            if (itemPanel)
            {
                UnityEngine.UI.Button itemBtn = itemPanel.GetComponent<UnityEngine.UI.Button>();
                if (itemBtn) itemBtn.onClick.AddListener(OnItemPanelClicked);
            }

            RefreshItemDisplay();
        }

        private void OnEnable()
        {
            if (gameState) gameState.OnItemChanged += RefreshItemDisplay;
        }

        private void OnDisable()
        {
            if (gameState) gameState.OnItemChanged -= RefreshItemDisplay;
        }

        public void ShowDialogue(string speaker, string text)
        {
            if (dialoguePanel) dialoguePanel.SetActive(true);
            if (speakerText) speakerText.text = speaker;
            if (dialogueText) dialogueText.text = text;
        }

        public void SetDialogueLayout(DialogueLayoutSettings settings)
        {
            if (dialogueRectTransform == null || settings == null) return;

            dialogueRectTransform.anchorMin = settings.anchorMin;
            dialogueRectTransform.anchorMax = settings.anchorMax;
            dialogueRectTransform.pivot = settings.pivot;
            dialogueRectTransform.anchoredPosition = settings.anchoredPosition;
            dialogueRectTransform.sizeDelta = settings.sizeDelta;
        }

        public void ResetDialogueLayout()
        {
            if (dialogueRectTransform == null) return;

            dialogueRectTransform.anchorMin = defaultAnchorMin;
            dialogueRectTransform.anchorMax = defaultAnchorMax;
            dialogueRectTransform.pivot = defaultPivot;
            dialogueRectTransform.anchoredPosition = defaultAnchoredPosition;
            dialogueRectTransform.sizeDelta = defaultSizeDelta;
        }

        public void HideDialogue()
{
            if (dialoguePanel) dialoguePanel.SetActive(false);
        }

        public void RefreshItemDisplay()
        {
            if (gameState == null) return;

            var item = gameState.activeItem;
            bool hasItem = item != null;

            // �A�C�e��������ꍇ�̂݃p�l����\������i�A�C�e�����Ȃ��ꍇ�͔�\���ɂ��Ȃ��j
            if (itemPanel && hasItem && !itemPanel.activeSelf)
            {
                itemPanel.SetActive(true);
            }

            if (activeItemImage) activeItemImage.sprite = hasItem ? item.icon : null;
            if (itemText) itemText.text = hasItem ? (item.itemName ?? string.Empty) : string.Empty;
        }

        public void OnItemPanelClicked()
        {
            if (inventoryUI) inventoryUI.Toggle();
        }

        public void ToggleSettings()
        {
            if (settingsPanel) settingsPanel.SetActive(!settingsPanel.activeSelf);
        }

        private void OnAdvanceClicked()
        {
            if (DialogueManager.Instance)
            {
                DialogueManager.Instance.AdvanceDialogue();
            }
        }
    }
}
