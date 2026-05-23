using UnityEngine;
using ShiraOzi.Core;
using System.Collections.Generic;

namespace ShiraOzi.UI
{
    /// <summary>
    /// インベントリ（所持品一覧）画面の表示と操作を管理するクラス。
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameState gameState;      // ゲーム状態データへの参照
        [SerializeField] private GameObject inventoryPanel; // インベントリ全体のパネル
        [SerializeField] private InventoryItemUI itemPrefab; // アイテム表示用の子要素プレハブ
        [SerializeField] private Transform itemContainer;    // アイテムを並べるコンテナ

        /// <summary>
        /// インベントリの開閉を切り替える。
        /// </summary>
        public void Toggle()
        {
            if (inventoryPanel.activeSelf)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        /// <summary>
        /// インベントリを開き、アイテム一覧を生成する。
        /// </summary>
        public void Open()
        {
            inventoryPanel.SetActive(true);
            Populate();
        }

        /// <summary>
        /// インベントリを閉じる。
        /// </summary>
        public void Close()
        {
            inventoryPanel.SetActive(false);
        }

        private void Update()
        {
            // インベントリ表示中にEscapeキーが押されたら閉じる
            if (inventoryPanel.activeSelf && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Close();
            }
        }

        /// <summary>
        /// 現在所持しているアイテムをUIに一覧表示する。
        /// </summary>
        private void Populate()
        {
            // 既存のアイテム要素をすべて削除
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }

            if (gameState == null) return;

            // 所持アイテムリストをループして要素を作成
            foreach (var item in gameState.acquiredItems)
            {
                InventoryItemUI itemUI = Instantiate(itemPrefab, itemContainer);
                itemUI.Initialize(item, OnItemClicked);
            }
        }

        /// <summary>
        /// アイテムがクリックされたときに呼び出される。
        /// クリックされたアイテムをアクティブにするか、解除する。
        /// </summary>
        /// <param name="item">クリックされたアイテムのデータ</param>
        private void OnItemClicked(ItemData item)
        {
            if (gameState.activeItem == item)
            {
                // すでにアクティブなアイテムなら解除
                gameState.SetActiveItem(null);
            }
            else
            {
                // 未アクティブなら設定
                gameState.SetActiveItem(item);
            }
            Close(); // アイテム選択後にインベントリを閉じる
        }
    }
}
