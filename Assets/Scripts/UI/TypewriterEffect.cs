using UnityEngine;
using TMPro;
using System.Collections;

namespace ShiraOzi.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterEffect : MonoBehaviour
    {
        [SerializeField] private float charactersPerSecond = 30f;

        private TMP_Text textComponent;
        private Coroutine typingCoroutine;

        public bool IsBusy { get; private set; }

        private void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
        }

        public void Play(string text)
        {
            if (IsBusy)
            {
                StopTyping();
            }

            textComponent.text = text;
            typingCoroutine = StartCoroutine(TypeText(text));
        }

        public void Skip()
        {
            if (!IsBusy) return;

            StopTyping();
            textComponent.maxVisibleCharacters = textComponent.text.Length;
        }

        private IEnumerator TypeText(string text)
        {
            IsBusy = true;
            textComponent.maxVisibleCharacters = 0;
            
            // Forces the text component to update its content to get correct character count
            textComponent.ForceMeshUpdate();
            int totalCharacters = textComponent.textInfo.characterCount;

            float duration = 1f / charactersPerSecond;
            int visibleCharacters = 0;

            while (visibleCharacters < totalCharacters)
            {
                visibleCharacters++;
                textComponent.maxVisibleCharacters = visibleCharacters;
                yield return new WaitForSeconds(duration);
            }

            IsBusy = false;
            typingCoroutine = null;
        }

        private void StopTyping()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            IsBusy = false;
        }
    }
}
