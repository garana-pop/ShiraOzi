using UnityEngine;
using ShiraOzi.Core;
using System.Collections.Generic;

namespace ShiraOzi.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private InventoryItemUI itemPrefab;
        [SerializeField] private Transform itemContainer;

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

        public void Open()
        {
            inventoryPanel.SetActive(true);
            Populate();
        }

        public void Close()
        {
            inventoryPanel.SetActive(false);
        }

        private void Update()
        {
            if (inventoryPanel.activeSelf && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Close();
            }
        }

        private void Populate()
{
            // Clear existing items
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }

            if (gameState == null) return;

            foreach (var item in gameState.acquiredItems)
            {
                InventoryItemUI itemUI = Instantiate(itemPrefab, itemContainer);
                itemUI.Initialize(item, OnItemClicked);
            }
        }

        private void OnItemClicked(ItemData item)
        {
            if (gameState.activeItem == item)
            {
                gameState.SetActiveItem(null);
            }
            else
            {
                gameState.SetActiveItem(item);
            }
            Close();
        }
}
}
