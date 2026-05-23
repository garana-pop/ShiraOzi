using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.Interaction
{
    /// <summary>
    /// アイテムの取得処理を管理するコンポーネント。
    /// </summary>
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private GameState gameState; // ゲーム状態データへの参照
        [SerializeField] private ItemData itemData;   // 取得するアイテムのデータ

        /// <summary>
        /// アイテムを取得し、インベントリに追加する。
        /// </summary>
        public void PickUp()
        {
            if (gameState && itemData != null)
            {
                // インベントリにアイテムを追加し、オブジェクトを非アクティブにする
                gameState.AddItem(itemData);
                Debug.Log($"Picked up: {itemData.itemName}");
                gameObject.SetActive(false); 
            }
        }
    }
}
