using UnityEngine;

namespace ShiraOzi.Core
{
    public class ChapterManager : MonoBehaviour
    {
        public static ChapterManager Instance { get; private set; }

        public GameState gameState;

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

        public void NextChapter()
        {
            if (gameState)
            {
                gameState.currentChapter++;
                Debug.Log($"Moved to Chapter: {gameState.currentChapter}");
            }
        }

        public void SetChapter(int chapter)
        {
            if (gameState)
            {
                gameState.currentChapter = chapter;
                Debug.Log($"Set Chapter: {gameState.currentChapter}");
            }
        }
    }
}
