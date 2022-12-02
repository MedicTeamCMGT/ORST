using System.Collections.Generic;
using ORST.Core.Dialogues;
using ORST.Foundation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ORST.Core {
    public class TutorialDialogueView : BaseMonoBehaviour {
        [SerializeField, Required] private TMP_Text m_Title;
        [SerializeField, Required] private TMP_Text m_Text;
        [SerializeField, Required] private List<Image> m_Images;

        private TutorialDialogueNode m_CurrentDialogueNode;
        
        
    }
}