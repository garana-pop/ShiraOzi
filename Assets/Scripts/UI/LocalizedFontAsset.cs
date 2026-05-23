using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace ShiraOzi.UI
{
    /// <summary>
    /// Swaps a TMP_Text font based on the active locale using an Asset Table.
    /// </summary>
    [AddComponentMenu("Localization/Asset/Localized Font Asset")]
    public class LocalizedFontAsset : LocalizedAssetBehaviour<TMP_FontAsset, LocalizedTmpFont>
    {
        protected override void UpdateAsset(TMP_FontAsset font)
        {
            var tmp = GetComponent<TMP_Text>();
            if (tmp != null && font != null)
            {
                tmp.font = font;
            }
        }
    }
}
