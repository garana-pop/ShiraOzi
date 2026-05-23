using UnityEngine;

namespace ShiraOzi.Core
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ShiraOzi/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemID;
        public string itemName;
        public Sprite icon;
    }
}
