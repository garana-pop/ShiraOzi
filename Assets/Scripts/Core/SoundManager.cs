using UnityEngine;

namespace ShiraOzi.Core
{
    /// <summary>
    /// ゲーム内のSE（効果音）の再生を管理するマネージャークラス。
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; } // シングルトンインスタンス

        [SerializeField] private AudioSource bgmSource; // BGM再生用のオーディオソース
        [SerializeField] private AudioSource sfxSource; // 効果音再生用のオーディオソース
        [SerializeField] private AudioClip hoverClip;   // ホバー時の効果音
        [SerializeField] private AudioClip clickClip;   // クリック時の効果音

        private float bgmVolume = 1f;
        private float sfxVolume = 1f;

        private void Awake()
        {
            // シングルトンの初期化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // 初期音量設定
                if (sfxSource != null) sfxSource.volume = sfxVolume;
                if (bgmSource != null) bgmSource.volume = bgmVolume;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// ホバー時の効果音を再生する。
        /// </summary>
        public void PlayHoverSound()
        {
            if (hoverClip && sfxSource)
            {
                Debug.Log("[SoundManager] PlayHoverSound: " + hoverClip.name);
                sfxSource.PlayOneShot(hoverClip);
            }
            else
            {
                Debug.LogWarning("[SoundManager] PlayHoverSound failed: hoverClip=" + hoverClip + ", sfxSource=" + sfxSource);
            }
        }

        /// <summary>
        /// クリック時の効果音を再生する。
        /// </summary>
        public void PlayClickSound()
        {
            if (clickClip && sfxSource)
            {
                Debug.Log("[SoundManager] PlayClickSound: " + clickClip.name);
                sfxSource.PlayOneShot(clickClip);
            }
            else
            {
                Debug.LogWarning("[SoundManager] PlayClickSound failed: clickClip=" + clickClip + ", sfxSource=" + sfxSource);
            }
        }

        /// <summary>
        /// BGMの音量を設定する。
        /// </summary>
        /// <param name="volume">音量（0.0〜1.0）</param>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            if (bgmSource) bgmSource.volume = bgmVolume;
        }

        /// <summary>
        /// SEの音量を設定する。
        /// </summary>
        /// <param name="volume">音量（0.0〜1.0）</param>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource) sfxSource.volume = sfxVolume;
        }
        }
        }
