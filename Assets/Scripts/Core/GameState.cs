using UnityEngine;
using System.Collections.Generic;

namespace ShiraOzi.Core
{
    [CreateAssetMenu(fileName = "GameState", menuName = "ShiraOzi/GameState")]
    public class GameState : ScriptableObject
    {
        [Header("Progress")]
        public int currentChapter = 1;

        [Header("Inventory")]
        public string heldItemID = "";

        [Header("Diary")]
        public List<string> unlockedDiaryEntries = new List<string>();

        public void ResetState()
        {
            currentChapter = 1;
            heldItemID = "";
            unlockedDiaryEntries.Clear();
        }

        public bool IsDiaryEntryUnlocked(string entryID)
        {
            return unlockedDiaryEntries.Contains(entryID);
        }

        public void UnlockDiaryEntry(string entryID)
        {
            if (!unlockedDiaryEntries.Contains(entryID))
            {
                unlockedDiaryEntries.Add(entryID);
            }
        }
    }
}
