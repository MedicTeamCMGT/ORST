using System;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [Serializable]
    public class TutorialDialogueNode {
        [SerializeField] private string m_Title;
        [SerializeField] private string m_Text;
        [SerializeField] private Sprite m_Image;

        public string Title => m_Title;
        public string Text => m_Text;
        public Sprite Image => m_Image;
    }
}