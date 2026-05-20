using UnityEngine;
using ShiraOzi.Core;

namespace ShiraOzi.Interaction
{
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private string itemID;

        public void PickUp()
        {
            if (gameState)
            {
                gameState.heldItemID = itemID;
                Debug.Log($"Picked up: {itemID}");
            }
        }
    }
}
