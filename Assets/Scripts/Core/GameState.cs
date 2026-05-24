using UnityEngine;
using System.Collections.Generic;

namespace ShiraOzi.Core
{
    /// <summary>
    /// ゲームの全体的な進行状況やプレイヤーの状態を保持するScriptableObject。
    /// </summary>
    [CreateAssetMenu(fileName = "GameState", menuName = "ShiraOzi/GameState")]
    public class GameState : ScriptableObject
    {
        [Header("Progress")]
        public int currentChapter = 1; // 現在のチャプター番号
        public bool hasSeenOpening = false; // オープニングを視聴済みかどうか

        [Header("Inventory")]
public string heldItemID = ""; // 現在手に持っているアイテムのID
        public List<ItemData> acquiredItems = new List<ItemData>(); // 獲得済みのアイテムリスト
        public ItemData activeItem; // 現在アクティブ（選択中）なアイテム

        public event System.Action OnItemChanged; // アイテムの状態が変更されたときに通知するイベント

        [Header("Diary")]
        public List<string> unlockedDiaryEntries = new List<string>(); // 解放済みのダイアリーエントリIDリスト

        /// <summary>
        /// ゲームの状態を初期値にリセットする。
        /// </summary>
        public void ResetState()
        {
            // すべての変数を初期状態に戻す
            currentChapter = 1;
            hasSeenOpening = false;
            heldItemID = "";
            acquiredItems.Clear();
            activeItem = null;
            unlockedDiaryEntries.Clear();
        }

        /// <summary>
        /// 指定されたダイアリーエントリが解放されているか確認する。
        /// </summary>
        /// <param name="entryID">確認するエントリのID</param>
        /// <returns>解放されている場合はtrue、そうでない場合はfalse</returns>
        public bool IsDiaryEntryUnlocked(string entryID)
        {
            return unlockedDiaryEntries.Contains(entryID);
        }

        /// <summary>
        /// 指定されたダイアリーエントリを解放する。
        /// </summary>
        /// <param name="entryID">解放するエントリのID</param>
        public void UnlockDiaryEntry(string entryID)
        {
            // 未解放の場合のみリストに追加
            if (!unlockedDiaryEntries.Contains(entryID))
            {
                unlockedDiaryEntries.Add(entryID);
            }
        }

        /// <summary>
        /// アイテムをインベントリに追加する。
        /// </summary>
        /// <param name="item">追加するアイテムのデータ</param>
        public void AddItem(ItemData item)
        {
            if (item == null) return;
            
            // リストに含まれていない場合に追加
            if (!acquiredItems.Contains(item))
            {
                acquiredItems.Add(item);
            }

            // アクティブアイテムがない場合は自動的に設定
            if (activeItem == null)
            {
                SetActiveItem(item);
            }
        }

        /// <summary>
        /// 指定されたアイテムをアクティブ状態に設定する。
        /// </summary>
        /// <param name="item">設定するアイテムのデータ</param>
        public void SetActiveItem(ItemData item)
        {
            activeItem = item;
            heldItemID = item != null ? item.itemID : "";
            OnItemChanged?.Invoke();
        }
    }
}
