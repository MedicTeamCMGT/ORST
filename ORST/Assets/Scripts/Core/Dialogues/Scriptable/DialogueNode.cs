using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class DialogueNode {
        [SerializeField, Required] private string m_Text;
        [SerializeField, Required] private List<DialogueOption> m_Options = new();

        public string Text => m_Text;
        public ReadOnlyCollection<DialogueOption> Options => m_Options.AsReadOnly();
    }
}