using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class DialogueOption {
        [SerializeField, Required] private string m_Text;
        [SerializeField] private bool m_IsCorrect;
        [SerializeField, Required, HideIf(nameof(m_IsCorrect))] private string m_HintText;

        public string Text => m_Text;
        public bool IsCorrect => m_IsCorrect;
        public string HintText => m_HintText;
    }
}