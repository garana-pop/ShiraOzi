using UnityEngine;
using System;

namespace ShiraOzi.Core
{
    [Serializable]
    public struct DialogueLine
    {
        public string speakerKey;
        public string textKey;
    }

    [CreateAssetMenu(fileName = "DialogueEntry", menuName = "ShiraOzi/DialogueEntry")]
    public class DialogueEntry : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}
