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
        [Header("Screen")]
        public UnityEngine.UI.Toggle fullscreenToggle; // 「フルスクリーン」ラジオトグル
        public UnityEngine.UI.Toggle windowedToggle;   // 「ウィンドウ」ラジオトグル
        public TMP_Dropdown windowSizeDropdown;        // ウィンドウサイズ選択ドロップダウン
        
        [Header("Audio")]
        public Slider bgmSlider; // BGM音量スライダー
        public Slider sfxSlider; // SE音量スライダー
        
        [Header("Localization")]
        public TMP_Dropdown languageDropdown; // 言語選択ドロップダウン
        
        [Header("Data Management")]
        public GameState gameState; // ゲーム状態データへの参照
        public GameObject resetConfirmationPanel; // リセット確認パネル

        // ウィンドウサイズの固定候補（ドロップダウンの表示順と一致）
        private static readonly Vector2Int[] WindowSizes =
        {
            new Vector2Int(1920, 1080),
            new Vector2Int(1280, 720),
            new Vector2Int(960, 540),
            new Vector2Int(640, 360),
        };

        private bool _isInitialized;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 設定の初期化を行う。
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            // 画面モード（フルスクリーン/ウィンドウ）ラジオの初期化
            bool isFullscreen = true;
            if (SaveManager.Instance != null)
            {
                isFullscreen = SaveManager.Instance.IsFullscreen;
            }
            else
            {
                isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ||
                               Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = isFullscreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }
            if (windowedToggle != null)
            {
                windowedToggle.isOn = !isFullscreen;
            }

            // ウィンドウサイズドロップダウンの初期化
            InitializeWindowSizeDropdown();

            // 現在の画面モードに合わせてドロップダウンの操作可否を設定
            UpdateWindowSizeInteractable(isFullscreen);

            // 音量スライダーの初期化
            InitializeAudioSliders();

            // 言語ドロップダウンの初期化
            InitializeLanguageDropdown();
        }

        private void InitializeWindowSizeDropdown()
        {
            if (windowSizeDropdown == null) return;

            windowSizeDropdown.ClearOptions();

            List<string> options = new List<string>();
            int currentIndex = 0;

            int targetWidth = Screen.width;
            int targetHeight = Screen.height;
            if (SaveManager.Instance != null)
            {
                targetWidth = SaveManager.Instance.ResolutionWidth;
                targetHeight = SaveManager.Instance.ResolutionHeight;
            }

            for (int i = 0; i < WindowSizes.Length; i++)
            {
                options.Add(WindowSizes[i].x + " x " + WindowSizes[i].y);

                // 現在のウィンドウサイズと一致する候補を初期選択
                if (WindowSizes[i].x == targetWidth && WindowSizes[i].y == targetHeight)
                {
                    currentIndex = i;
                }
            }

            windowSizeDropdown.AddOptions(options);
            windowSizeDropdown.SetValueWithoutNotify(currentIndex);
            windowSizeDropdown.RefreshShownValue();
            windowSizeDropdown.onValueChanged.AddListener(SetWindowSize);
        }

        private void InitializeAudioSliders()
        {
            if (bgmSlider != null)
            {
                if (SoundManager.Instance != null && SoundManager.Instance.IsVolumeLoaded)
                {
                    bgmSlider.value = SoundManager.Instance.BGMVolume;
                }
                bgmSlider.onValueChanged.AddListener(SetBGMVolume);
                SetBGMVolume(bgmSlider.value);
            }
            if (sfxSlider != null)
            {
                if (SoundManager.Instance != null && SoundManager.Instance.IsVolumeLoaded)
                {
                    sfxSlider.value = SoundManager.Instance.SFXVolume;
                }
                sfxSlider.onValueChanged.AddListener(SetSFXVolume);
                SetSFXVolume(sfxSlider.value);
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
        /// フルスクリーン/ウィンドウを切り替える。「フルスクリーン」ラジオの値変更で呼ばれる。
        /// </summary>
        public void SetFullscreen(bool isFullscreen)
        {
            if (SaveManager.Instance != null)
            {
                int width = SaveManager.Instance.ResolutionWidth;
                int height = SaveManager.Instance.ResolutionHeight;
                if (!isFullscreen && windowSizeDropdown != null)
                {
                    int index = windowSizeDropdown.value;
                    if (index >= 0 && index < WindowSizes.Length)
                    {
                        width = WindowSizes[index].x;
                        height = WindowSizes[index].y;
                    }
                }
                SaveManager.Instance.SetScreenSettings(isFullscreen, width, height);
            }
            else
            {
                if (isFullscreen)
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
                else
                {
                    // ウィンドウ化し、ドロップダウンで選択中のサイズを適用する。
                    ApplyWindowSize(windowSizeDropdown != null ? windowSizeDropdown.value : 0);
                }
            }

            // フルスクリーン時はウィンドウサイズドロップダウンを操作不可（グレー表示）にする。
            UpdateWindowSizeInteractable(isFullscreen);
        }

        /// <summary>
        /// ドロップダウンで選択されたウィンドウサイズを適用する。
        /// フルスクリーン中は何もしない。
        /// </summary>
        public void SetWindowSize(int index)
        {
            bool isCurrentlyFullscreen = false;
            if (SaveManager.Instance != null)
            {
                isCurrentlyFullscreen = SaveManager.Instance.IsFullscreen;
            }
            else
            {
                isCurrentlyFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ||
                                        Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
            }

            if (isCurrentlyFullscreen)
            {
                return;
            }

            if (index < 0 || index >= WindowSizes.Length) return;
            Vector2Int size = WindowSizes[index];

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SetScreenSettings(false, size.x, size.y);
            }
            else
            {
                ApplyWindowSize(index);
            }
        }

        private void ApplyWindowSize(int index)
        {
            if (index < 0 || index >= WindowSizes.Length) return;
            Vector2Int size = WindowSizes[index];
            Screen.SetResolution(size.x, size.y, FullScreenMode.Windowed);
        }

        /// <summary>
        /// ウィンドウサイズドロップダウンの操作可否を更新する。
        /// フルスクリーン時は interactable=false となり、Unity の Disabled カラーでグレー表示になる。
        /// </summary>
        private void UpdateWindowSizeInteractable(bool isFullscreen)
        {
            if (windowSizeDropdown != null)
            {
                windowSizeDropdown.interactable = !isFullscreen;
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
