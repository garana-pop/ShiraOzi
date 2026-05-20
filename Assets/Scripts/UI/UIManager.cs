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
        public TextMeshProUGUI itemText;

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

        public void UpdateHeldItem(string itemName)
        {
            if (itemPanel) itemPanel.SetActive(!string.IsNullOrEmpty(itemName));
            if (itemText) itemText.text = itemName;
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
