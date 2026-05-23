using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button button;

        private ItemData itemData;
        private System.Action<ItemData> onClickAction;

        public void Initialize(ItemData item, System.Action<ItemData> onClick)
        {
            itemData = item;
            onClickAction = onClick;

            if (iconImage) iconImage.sprite = item.icon;
            if (nameText) nameText.text = item.itemName;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickAction?.Invoke(itemData));
        }
    }
}
