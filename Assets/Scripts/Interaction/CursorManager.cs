using UnityEngine;

namespace ShiraOzi.Interaction
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D interactCursor;
        [SerializeField] private Vector2 hotspot = Vector2.zero;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetDefaultCursor()
        {
            Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        }

        public void SetInteractCursor()
        {
            Cursor.SetCursor(interactCursor, hotspot, CursorMode.Auto);
        }
    }
}
