using System;
using UnityEngine;

namespace ShiraOzi.Core
{
    /// <summary>
    /// textKey のサフィックス（_Ozi, _Protagonist, _Narration 等）と
    /// 話者名（ローカライズキー）の対応を管理する ScriptableObject。
    /// </summary>
    [CreateAssetMenu(fileName = "SpeakerMapping", menuName = "ShiraOzi/SpeakerMapping")]
    public class SpeakerMapping : ScriptableObject
    {
        [Serializable]
        public struct SpeakerEntry
        {
            [Tooltip("textKey の最後の '_' 以降のサフィックス。例: Ozi, Protagonist, Narration")]
            public string suffix;

            [Tooltip("話者名のローカライズキー（UIStrings テーブル）。hideSpeaker が true の場合は無視。")]
            public string speakerNameKey;

            [Tooltip("true の場合、話者ラベルを非表示にする（ナレーション用）。")]
            public bool hideSpeaker;
        }

        public SpeakerEntry[] entries;

        /// <summary>
        /// サフィックスから対応エントリを取得する（大文字小文字を無視）。
        /// </summary>
        /// <param name="suffix">textKey から抽出したサフィックス</param>
        /// <param name="result">見つかったエントリ</param>
        /// <returns>対応エントリが存在すれば true</returns>
        public bool TryGet(string suffix, out SpeakerEntry result)
        {
            if (!string.IsNullOrEmpty(suffix) && entries != null)
            {
                foreach (var e in entries)
                {
                    if (string.Equals(e.suffix, suffix, StringComparison.OrdinalIgnoreCase))
                    {
                        result = e;
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }
    }
}
