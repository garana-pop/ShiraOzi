using UnityEngine;
using ShiraOzi.Core;
using System.Collections.Generic;

namespace ShiraOzi.UI
{
    /// <summary>
    /// ダイアリー（日誌）画面の表示と解放済みエントリの管理を行うクラス。
    /// </summary>
    public class DiaryManager : MonoBehaviour
    {
        public GameState gameState;      // ゲーム状態データへの参照
        public GameObject diaryPanel;    // ダイアリー表示パネル
        public GameObject entryPrefab;   // エントリ表示用のプレハブ
        public Transform contentParent;  // エントリを配置するコンテナ

        /// <summary>
        /// ダイアリー画面を表示する。
        /// </summary>
        public void ShowDiary()
        {
            if (diaryPanel) diaryPanel.SetActive(true);
            PopulateEntries();
        }

        /// <summary>
        /// ダイアリー画面を非表示にする。
        /// </summary>
        public void HideDiary()
        {
            if (diaryPanel) diaryPanel.SetActive(false);
        }

        /// <summary>
        /// 解放されているダイアリーエントリを一覧表示する。
        /// </summary>
        private void PopulateEntries()
        {
            if (contentParent == null) return;

            // 既存のエントリUIをすべて削除
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            if (gameState == null) return;

            // 解放済みのエントリIDリストをループしてUIを生成
            foreach (string entryID in gameState.unlockedDiaryEntries)
            {
                if (entryPrefab)
                {
                    GameObject obj = Instantiate(entryPrefab, contentParent);
                    // 必要に応じてここでエントリUIの内容（テキスト等）を更新
                }
            }
        }
    }
}
