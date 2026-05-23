using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace ShiraOzi.UI
{
    /// <summary>
    /// 現在のロケール（言語設定）に基づいて、TextMeshProのフォントアセットを動的に差し替えるクラス。
    /// </summary>
    [AddComponentMenu("Localization/Asset/Localized Font Asset")]
    public class LocalizedFontAsset : LocalizedAssetBehaviour<TMP_FontAsset, LocalizedTmpFont>
    {
        /// <summary>
        /// アセットが更新されたときに呼び出され、TMP_Textに新しいフォントを適用する。
        /// </summary>
        /// <param name="font">新しく適用されるフォントアセット</param>
        protected override void UpdateAsset(TMP_FontAsset font)
        {
            var tmp = GetComponent<TMP_Text>();
            if (tmp != null && font != null)
            {
                // テキストコンポーネントのフォントを差し替え
                tmp.font = font;
            }
        }
    }
}
