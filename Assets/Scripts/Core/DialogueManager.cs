using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using ShiraOzi.UI;
using System.Collections;

namespace ShiraOzi.Core
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [SerializeField] private GameState gameState;
        
        private bool isDisplaying;
        private DialogueEntry currentEntry;
        private int currentLineIndex;

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

        public void StartDialogue(DialogueEntry entry)
        {
            if (entry == null || isDisplaying) return;
            
            currentEntry = entry;
            currentLineIndex = 0;
            isDisplaying = true;
            
            DisplayCurrentLine();
        }

        public void AdvanceDialogue()
        {
            if (!isDisplaying) return;

            currentLineIndex++;
            if (currentLineIndex < currentEntry.lines.Length)
            {
                DisplayCurrentLine();
            }
            else
            {
                EndDialogue();
            }
        }

        private void DisplayCurrentLine()
        {
            var line = currentEntry.lines[currentLineIndex];
            
            string speaker = GetLocalizedString("UIStrings", line.speakerKey);
            string text = GetLocalizedString("UIStrings", line.textKey);
            
            if (UIManager.Instance)
            {
                UIManager.Instance.ShowDialogue(speaker, text);
            }
        }

        private void EndDialogue()
        {
            isDisplaying = false;
            if (UIManager.Instance)
            {
                UIManager.Instance.HideDialogue();
            }
        }

        private string GetLocalizedString(string tableReference, string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableReference, key);
        }
    }
}
