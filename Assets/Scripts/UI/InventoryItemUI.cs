using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    /// <summary>
    /// インベントリ内の個々のアイテム要素を管理するクラス。
    /// </summary>
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;       // アイテムアイコンの表示用
        [SerializeField] private TextMeshProUGUI nameText; // アイテム名の表示用
        [SerializeField] private Button button;           // クリック判定用ボタン

        private ItemData itemData; // 保持しているアイテムデータ
        private System.Action<ItemData> onClickAction; // クリック時に実行されるアクション

        /// <summary>
        /// アイテムUIの初期設定を行う。
        /// </summary>
        /// <param name="item">表示するアイテムデータ</param>
        /// <param name="onClick">クリック時に呼び出すコールバック</param>
        public void Initialize(ItemData item, System.Action<ItemData> onClick)
        {
            itemData = item;
            onClickAction = onClick;

            // アイコンと名前を設定
            if (iconImage) iconImage.sprite = item.icon;
            if (nameText) nameText.text = item.itemName;

            // ボタンのリスナーをリセットして再登録
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickAction?.Invoke(itemData));
        }
    }
}
