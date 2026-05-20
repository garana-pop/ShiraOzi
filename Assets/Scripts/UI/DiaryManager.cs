using UnityEngine;
using ShiraOzi.Core;
using System.Collections.Generic;

namespace ShiraOzi.UI
{
    public class DiaryManager : MonoBehaviour
    {
        public GameState gameState;
        public GameObject diaryPanel;
        public GameObject entryPrefab;
        public Transform contentParent;

        public void ShowDiary()
        {
            if (diaryPanel) diaryPanel.SetActive(true);
            PopulateEntries();
        }

        public void HideDiary()
        {
            if (diaryPanel) diaryPanel.SetActive(false);
        }

        private void PopulateEntries()
        {
            if (contentParent == null) return;

            // Clear old entries
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            if (gameState == null) return;

            // Create new ones based on gameState.unlockedDiaryEntries
            foreach (string entryID in gameState.unlockedDiaryEntries)
            {
                if (entryPrefab)
                {
                    GameObject obj = Instantiate(entryPrefab, contentParent);
                    // Update entry UI logic here
                }
            }
        }
    }
}
