using System;
using System.Collections.Generic;

namespace ShiraOzi.Core
{
    /// <summary>
    /// ゲームのセーブデータを保持するシリアライズ可能なクラス。
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int currentChapter;
        public bool hasSeenOpening;
        public string heldItemID;
        public List<string> acquiredItemIDs;
        public List<string> unlockedDiaryEntries;
        public float bgmVolume;
        public float sfxVolume;
        public bool isFullscreen;
        public int resolutionWidth;
        public int resolutionHeight;

        public SaveData()
        {
            acquiredItemIDs = new List<string>();
            unlockedDiaryEntries = new List<string>();
            bgmVolume = 1f;
            sfxVolume = 1f;
            isFullscreen = true;
            resolutionWidth = 1920;
            resolutionHeight = 1080;
        }
    }
}
