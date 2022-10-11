using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ORST.Core.Dialogues {
    public class DialogueButton : BaseMonoBehaviour {
        [SerializeField, Required] private TextMeshProUGUI m_Text;
        [SerializeField, Required] private Button m_Button;

        public TextMeshProUGUI Text => m_Text;
        public Button Button => m_Button;
    }
}