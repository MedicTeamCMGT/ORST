using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class TutorialDialogueNode {
        [SerializeField] private string m_Title;
        [SerializeField, TextArea(5,15)] private string m_Text;
        [SerializeField] private Sprite[] m_Images;
        [SerializeField] private bool m_ShowPopup;
        [SerializeField, ShowIf(nameof(m_ShowPopup))] private string m_PopupTitle;
        [SerializeField, ShowIf(nameof(m_ShowPopup)), TextArea(3, 10)] private string m_PopupText;
        [SerializeField, ShowIf(nameof(m_ShowPopup))] private bool m_SnapPopupToPosition;
        [SerializeField, ShowIf(nameof(m_SnapPopupToPosition))] private Transform m_SnapPosition;

        public string Title => m_Title;
        public string Text => m_Text;
        public Sprite[] Images => m_Images;
        public bool ShowPopup => m_ShowPopup;
        public string PopupTitle => m_PopupTitle;
        public string PopupText => m_PopupText;
        public bool SnapPopupToPosition => m_SnapPopupToPosition;
        public Transform SnapPosition => m_SnapPosition;
    }
}