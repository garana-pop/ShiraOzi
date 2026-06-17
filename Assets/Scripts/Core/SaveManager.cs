using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace ShiraOzi.Core
{
    /// <summary>
    /// ゲームのセーブとロード、および終了時の自動セーブを管理するクラス。
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        [Header("Data References")]
        public GameState gameState;
        public List<ItemData> allItems = new List<ItemData>();

        private string savePath;
        private bool preventSaveOnQuit = false;

        public bool IsFullscreen { get; private set; } = true;
        public int ResolutionWidth { get; private set; } = 1920;
        public int ResolutionHeight { get; private set; } = 1080;

        private void Awake()
        {
            InitializePath();

            // シングルトンの初期化
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // 起動時にロードを実行
            Load();
        }

        private void InitializePath()
        {
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = Path.Combine(Application.persistentDataPath, "save.json");
            }
        }

        private void OnApplicationQuit()
        {
            if (preventSaveOnQuit) return;
            // アプリケーション終了時に自動セーブ
            Save();
        }

        /// <summary>
        /// 現在のゲーム状態をJSON形式で保存する。
        /// </summary>
        public void Save()
        {
            if (gameState == null) return;
            InitializePath();

            SaveData data = new SaveData();
data.currentChapter = gameState.currentChapter;
            data.hasSeenOpening = gameState.hasSeenOpening;
            data.heldItemID = gameState.heldItemID;

            foreach (var item in gameState.acquiredItems)
            {
                if (item != null)
                {
                    data.acquiredItemIDs.Add(item.itemID);
                }
            }

            data.unlockedDiaryEntries = new List<string>(gameState.unlockedDiaryEntries);

            if (SoundManager.Instance != null)
            {
                data.bgmVolume = SoundManager.Instance.BGMVolume;
                data.sfxVolume = SoundManager.Instance.SFXVolume;
            }

            // 画面設定の保存（現在の意図した設定値を書き出し）
            data.isFullscreen = IsFullscreen;
            data.resolutionWidth = ResolutionWidth;
            data.resolutionHeight = ResolutionHeight;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"Game saved to: {savePath}");
        }

        /// <summary>
        /// セーブファイルを読み込み、ゲーム状態を復元する。
        /// </summary>
        public void Load()
        {
            if (gameState == null) return;
            InitializePath();

            if (File.Exists(savePath))
{
                string json = File.ReadAllText(savePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // GameStateの値を更新
                gameState.currentChapter = data.currentChapter;
                gameState.hasSeenOpening = data.hasSeenOpening;
                gameState.unlockedDiaryEntries = new List<string>(data.unlockedDiaryEntries);

                // 音量の復元
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.SetBGMVolume(data.bgmVolume);
                    SoundManager.Instance.SetSFXVolume(data.sfxVolume);
                    SoundManager.Instance.IsVolumeLoaded = true;
                }

                // 画面設定の読み込みと適用
                SetScreenSettings(data.isFullscreen, data.resolutionWidth, data.resolutionHeight);

                // アイテムリストの復元
                gameState.acquiredItems.Clear();
                foreach (string id in data.acquiredItemIDs)
                {
                    ItemData item = FindItemByID(id);
                    if (item != null)
                    {
                        gameState.acquiredItems.Add(item);
                    }
                }

                // アクティブアイテムの設定
                if (!string.IsNullOrEmpty(data.heldItemID))
                {
                    ItemData activeItem = FindItemByID(data.heldItemID);
                    if (activeItem != null)
                    {
                        gameState.SetActiveItem(activeItem);
                    }
                }
                else
                {
                    gameState.SetActiveItem(null);
                }

                Debug.Log("Game loaded successfully.");
            }
            else
            {
                Debug.Log("No save file found. Starting with default state.");
                // デフォルトはフルスクリーン
                SetScreenSettings(true, 1920, 1080);

                // セーブデータがない場合は初期値をセットして初プレイ状態にする
                gameState.ResetState(); 
            }
            }

            /// <summary>
            /// IDからItemDataアセットを検索する。
            /// </summary>
            private ItemData FindItemByID(string id)
            {
            if (allItems == null) return null;
            return allItems.Find(item => item != null && item.itemID == id);
            }

            /// <summary>
            /// 画面設定をゲームに適用し、内部状態を更新する。
            /// </summary>
            public void SetScreenSettings(bool isFullscreen, int width, int height)
            {
                IsFullscreen = isFullscreen;
                ResolutionWidth = width;
                ResolutionHeight = height;

                if (isFullscreen)
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
                else
                {
                    Screen.SetResolution(width, height, FullScreenMode.Windowed);
                }
                Debug.Log($"Applied screen settings: Fullscreen={IsFullscreen}, Resolution={ResolutionWidth}x{ResolutionHeight}");
            }

        /// <summary>
        /// セーブデータを削除する（開発・デバッグ用）。
        /// </summary>
        public void DeleteSaveData()
        {
            preventSaveOnQuit = true;
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save data deleted.");
            }
        }
    }
}
