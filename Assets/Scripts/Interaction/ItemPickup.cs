using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.Interaction
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private ItemData itemData;

        public void PickUp()
        {
            if (gameState && itemData != null)
            {
                gameState.AddItem(itemData);
                Debug.Log($"Picked up: {itemData.itemName}");
                gameObject.SetActive(false); // Disable after pickup
            }
        }
    }
}
