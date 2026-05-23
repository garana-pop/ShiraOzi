using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ShiraOzi.UI
{
    /// <summary>
    /// ゲームの環境設定（画面解像度、フルスクリーンなど）を管理するクラス。
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public UnityEngine.UI.Toggle fullscreenToggle; // フルスクリーン切り替え用トグル
        public TMP_Dropdown resolutionDropdown;        // 解像度選択用ドロップダウン

        private Resolution[] resolutions; // 利用可能な解像度のリスト

        private void Start()
        {
            // フルスクリーン設定の初期化
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = Screen.fullScreen;
                fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            }

            // 利用可能な解像度を取得してドロップダウンを構築
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

                    // 現在の解像度と一致するインデックスを保存
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

        /// <summary>
        /// フルスクリーン状態を切り替える。
        /// </summary>
        /// <param name="isFullscreen">フルスクリーンの場合はtrue</param>
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        /// <summary>
        /// 指定されたインデックスの解像度を設定する。
        /// </summary>
        /// <param name="resolutionIndex">解像度リストのインデックス</param>
        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
            {
                Resolution resolution = resolutions[resolutionIndex];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
    }
}
