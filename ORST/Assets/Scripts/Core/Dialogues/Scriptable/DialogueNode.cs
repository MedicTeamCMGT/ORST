using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class DialogueNode {
        [SerializeField, Required] private string m_Text;
        [SerializeField, Required] private List<DialogueOption> m_Options = new();

        public string Text => m_Text;
        public List<DialogueOption> Options => m_Options;
    }
}