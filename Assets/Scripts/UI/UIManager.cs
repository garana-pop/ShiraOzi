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

        private void Awake()
        {
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

        public void HideDialogue()
        {
            if (dialoguePanel) dialoguePanel.SetActive(false);
        }

        public void RefreshItemDisplay()
        {
            if (gameState == null) return;

            bool hasItem = gameState.activeItem != null;
            if (itemPanel) itemPanel.SetActive(hasItem);

            if (hasItem)
            {
                if (activeItemImage) activeItemImage.sprite = gameState.activeItem.icon;
                if (itemText) itemText.text = gameState.activeItem.itemName;
            }
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
