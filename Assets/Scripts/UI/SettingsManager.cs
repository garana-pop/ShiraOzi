using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ShiraOzi.Core;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ShiraOzi.UI
{
    /// <summary>
    /// ゲームの環境設定（画面解像度、音量、言語など）を管理するクラス。
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public UnityEngine.UI.Toggle fullscreenToggle; // フルスクリーン切り替え用トグル
        public TMP_Dropdown resolutionDropdown;        // 解像度選択用ドロップダウン
        
        [Header("Audio")]
        public Slider bgmSlider; // BGM音量スライダー
        public Slider sfxSlider; // SE音量スライダー
        
        [Header("Localization")]
        public TMP_Dropdown languageDropdown; // 言語選択ドロップダウン
        
        [Header("Data Management")]
        public GameState gameState; // ゲーム状態データへの参照
        public GameObject resetConfirmationPanel; // リセット確認パネル

        private Resolution[] resolutions; // 利用可能な解像度のリスト

        private void Start()
        {
            // フルスクリーン設定の初期化
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }

            // 解像度ドロップダウンの初期化
            InitializeResolutionDropdown();

            // 音量スライダーの初期化
            InitializeAudioSliders();

            // 言語ドロップダウンの初期化
            InitializeLanguageDropdown();
        }

        private void InitializeResolutionDropdown()
        {
            resolutions = Screen.resolutions;
            if (resolutionDropdown != null && resolutions.Length > 0)
            {
                resolutionDropdown.ClearOptions();

                List<string> options = new List<string>();
                int currentResolutionIndex = 0;
                for (int i = 0; i < resolutions.Length; i++)
                {
                    string option = resolutions[i].width + " x " + resolutions[i].height;
                    options.Add(option);

                    if (resolutions[i].width == Screen.currentResolution.width &&
                        resolutions[i].height == Screen.currentResolution.height)
                    {
                        currentResolutionIndex = i;
                    }
                }
                resolutionDropdown.AddOptions(options);
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
        }

        private void InitializeAudioSliders()
        {
            if (bgmSlider != null)
            {
                bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            }
            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            }
        }

        private void InitializeLanguageDropdown()
        {
            if (languageDropdown != null)
            {
                languageDropdown.ClearOptions();
                List<string> options = new List<string>();
                int currentLocaleIndex = 0;
                var locales = LocalizationSettings.AvailableLocales.Locales;

                for (int i = 0; i < locales.Count; i++)
                {
                    options.Add(locales[i].name);
                    if (locales[i] == LocalizationSettings.SelectedLocale)
{
                        currentLocaleIndex = i;
                    }
                }

                languageDropdown.AddOptions(options);
                languageDropdown.value = currentLocaleIndex;
                languageDropdown.RefreshShownValue();
                languageDropdown.onValueChanged.AddListener(SetLanguage);
            }
        }

        /// <summary>
        /// フルスクリーン状態を切り替える。
        /// </summary>
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        /// <summary>
        /// 指定されたインデックスの解像度を設定する。
        /// </summary>
        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
            {
                Resolution resolution = resolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }

        /// <summary>
        /// BGMの音量を設定する。
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            if (SoundManager.Instance) SoundManager.Instance.SetBGMVolume(volume);
        }

        /// <summary>
        /// SEの音量を設定する。
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            if (SoundManager.Instance) SoundManager.Instance.SetSFXVolume(volume);
        }

        /// <summary>
        /// 指定されたインデックスの言語に切り替える。
        /// </summary>
        public void SetLanguage(int index)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;
            if (index >= 0 && index < locales.Count)
            {
                LocalizationSettings.SelectedLocale = locales[index];
            }
        }

        /// <summary>
        /// データリセットの確認パネルを表示する。
        /// </summary>
        public void ShowResetConfirmation()
        {
            if (resetConfirmationPanel) resetConfirmationPanel.SetActive(true);
        }

        /// <summary>
        /// データを初期化し、確認パネルを閉じる。
        /// </summary>
        public void ConfirmReset()
        {
            if (gameState) gameState.ResetState();
            if (resetConfirmationPanel) resetConfirmationPanel.SetActive(false);
        }

        /// <summary>
        /// リセット確認パネルを閉じる（キャンセル）。
        /// </summary>
        public void CancelReset()
        {
            if (resetConfirmationPanel) resetConfirmationPanel.SetActive(false);
        }
    }
}
