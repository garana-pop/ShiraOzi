using UnityEngine;

namespace ShiraOzi.Core
{
    /// <summary>
    /// アイテムの基本データを保持するScriptableObject。
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "ShiraOzi/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemID;   // アイテムの一意識別ID
        public string itemName; // アイテムの名称
        public Sprite icon;     // インベントリに表示するアイコン画像
    }
}
