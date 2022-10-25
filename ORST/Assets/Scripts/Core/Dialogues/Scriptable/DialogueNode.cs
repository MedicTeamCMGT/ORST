using System;
using System.Collections.Generic;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class DialogueNode {
        [SerializeField] private string m_Text;
        [SerializeField] private List<DialogueOption> m_Options = new();

        public string Text => m_Text;
        public List<DialogueOption> Options => m_Options;

#if UNITY_EDITOR
        public const string TEXT_FIELD_NAME = nameof(m_Text);
        public const string OPTIONS_FIELD_NAME = nameof(m_Options);
#endif
    }
}