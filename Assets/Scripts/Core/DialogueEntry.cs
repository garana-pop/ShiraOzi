using UnityEngine;
using System;

namespace ShiraOzi.Core
{
    /// <summary>
    /// 会話の1行分のデータを定義する構造体。
    /// </summary>
    [Serializable]
    public struct DialogueLine
    {
        public string speakerKey; // 話者のローカライズキー
        public string textKey;    // セリフのローカライズキー
    }

    /// <summary>
    /// 一連の会話データを保持するScriptableObject。
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueEntry", menuName = "ShiraOzi/DialogueEntry")]
    public class DialogueEntry : ScriptableObject
    {
        public DialogueLine[] lines; // 会話行の配列
    }
}
