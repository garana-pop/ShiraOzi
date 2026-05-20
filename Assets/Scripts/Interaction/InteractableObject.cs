using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ShiraOzi.Core;

namespace ShiraOzi.Interaction
{
public class InteractableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityEvent onHoverEnter;
        public UnityEvent onHoverExit;
        public UnityEvent onClick;

        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverEnter?.Invoke();
            if (CursorManager.Instance) CursorManager.Instance.SetInteractCursor();
            if (SoundManager.Instance) SoundManager.Instance.PlayHoverSound();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverExit?.Invoke();
            if (CursorManager.Instance) CursorManager.Instance.SetDefaultCursor();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                onClick?.Invoke();
                if (SoundManager.Instance) SoundManager.Instance.PlayClickSound();
            }
        }
    }
}
