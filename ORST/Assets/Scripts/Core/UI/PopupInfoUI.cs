using TMPro;
using UnityEngine;

namespace ORST.Core.UI {
    public class PopupInfoUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_InfoTitle;
        [SerializeField] private TMP_Text m_InfoMessage;

        public void SetInfoTitle(string infoTitle) {
            m_InfoTitle.text = infoTitle;
        }

        public void SetInfoMessage(string infoMessage) {
            m_InfoMessage.text = infoMessage;
        }
    }
}
