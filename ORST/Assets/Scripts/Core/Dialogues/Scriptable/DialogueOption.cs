using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class DialogueOption {
        [SerializeField] private string m_Text;
        [SerializeField] private bool m_IsCorrect;
        [SerializeField, HideIf(nameof(m_IsCorrect))] private string m_Feedback;

        public string Text {
            get => m_Text;
            set => m_Text = value;
        }

        public bool IsCorrect {
            get => m_IsCorrect;
            set => m_IsCorrect = value;
        }

        public string Feedback {
            get => m_Feedback;
            set => m_Feedback = value;
        }
    }
}