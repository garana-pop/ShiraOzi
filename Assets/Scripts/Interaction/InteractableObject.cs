using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ShiraOzi.Core;

namespace ShiraOzi.Interaction
{
    /// <summary>
    /// マウス入力によるインタラクションを処理するコンポーネント。
    /// オブジェクトのホバーやクリックに対してイベントを発行する。
    /// </summary>
    public class InteractableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityEvent onHoverEnter; // マウスが重なったときのイベント
        public UnityEvent onHoverExit;  // マウスが離れたときのイベント
        public UnityEvent onClick;      // クリックされたときのイベント

        /// <summary>
        /// マウスカーソルがオブジェクトに入ったときに呼び出される。
        /// </summary>
        /// <param name="eventData">ポインターイベントのデータ</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverEnter?.Invoke();
            // カーソルをインタラクト用に変更し、ホバー音を再生
            if (CursorManager.Instance) CursorManager.Instance.SetInteractCursor();
            if (SoundManager.Instance) SoundManager.Instance.PlayHoverSound();
        }

        /// <summary>
        /// マウスカーソルがオブジェクトから出たときに呼び出される。
        /// </summary>
        /// <param name="eventData">ポインターイベントのデータ</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExit?.Invoke();
            // カーソルをデフォルトに戻す
            if (CursorManager.Instance) CursorManager.Instance.SetDefaultCursor();
        }

        /// <summary>
        /// オブジェクトがクリックされたときに呼び出される。
        /// </summary>
        /// <param name="eventData">ポインターイベントのデータ</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 左クリックの場合のみ処理を実行
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onClick?.Invoke();
                if (SoundManager.Instance) SoundManager.Instance.PlayClickSound();
            }
        }
    }
}
