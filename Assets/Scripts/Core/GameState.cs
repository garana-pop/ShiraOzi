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
        public List<ItemData> acquiredItems = new List<ItemData>();
        public ItemData activeItem;

        public event System.Action OnItemChanged;

        [Header("Diary")]
public List<string> unlockedDiaryEntries = new List<string>();

        public void ResetState()
        {
            currentChapter = 1;
            heldItemID = "";
            acquiredItems.Clear();
            activeItem = null;
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

        public void AddItem(ItemData item)
        {
            if (item == null) return;
            
            if (!acquiredItems.Contains(item))
            {
                acquiredItems.Add(item);
            }

            if (activeItem == null)
            {
                SetActiveItem(item);
            }
        }

        public void SetActiveItem(ItemData item)
        {
            activeItem = item;
            heldItemID = item != null ? item.itemID : "";
            OnItemChanged?.Invoke();
        }
        }
        }
