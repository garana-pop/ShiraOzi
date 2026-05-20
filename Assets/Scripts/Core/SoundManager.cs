using UnityEngine;

namespace ShiraOzi.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip clickClip;

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

        public void PlayHoverSound()
        {
            if (hoverClip && sfxSource) sfxSource.PlayOneShot(hoverClip);
        }

        public void PlayClickSound()
        {
            if (clickClip && sfxSource) sfxSource.PlayOneShot(clickClip);
        }
    }
}
