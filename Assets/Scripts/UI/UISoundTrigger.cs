using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ShiraOzi.Core;

namespace ShiraOzi.UI
{
    /// <summary>
    /// UI要素（Button, Toggle, Slider）のインタラクションに応じて効果音を再生するスクリプト。
    /// </summary>
    public class UISoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private Button button;
        private Toggle toggle;
        private Slider slider;

        private void Awake()
        {
            button = GetComponent<Button>();
            toggle = GetComponent<Toggle>();
            slider = GetComponent<Slider>();
        }

        /// <summary>
        /// マウスカーソルが要素の上に乗った時の処理。
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // ButtonまたはToggleかつ、インタラクト可能な場合のみホバー音を再生
            bool isInteractable = (button != null && button.interactable) || (toggle != null && toggle.interactable);
            
            if (isInteractable)
            {
                if (SoundManager.Instance != null)
                {
                    Debug.Log("[UISoundTrigger] Playing Hover Sound on " + name);
                    SoundManager.Instance.PlayHoverSound();
                }
                else
                {
                    Debug.LogWarning("[UISoundTrigger] SoundManager.Instance is null on OnPointerEnter (" + name + ")");
                }
            }
        }

        /// <summary>
        /// マウスが押された時の処理。
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            // インタラクト可能な場合のみクリック音を再生
            bool isInteractable = (button != null && button.interactable) ||
                                  (toggle != null && toggle.interactable) ||
                                  (slider != null && slider.interactable);

            if (isInteractable)
            {
                if (SoundManager.Instance != null)
                {
                    Debug.Log("[UISoundTrigger] Playing Click Sound on " + name);
                    SoundManager.Instance.PlayClickSound();
                }
                else
                {
                    Debug.LogWarning("[UISoundTrigger] SoundManager.Instance is null on OnPointerDown (" + name + ")");
                }
            }
        }
    }
}
