using ORST.Foundation;
using UnityEngine;
using UnityEngine.UI;

namespace ORST.Core.UI.Components {
    [RequireComponent(typeof(Button), typeof(CanvasGroup))]
    public class VRButton : BaseMonoBehaviour {
        private Button m_Button;
        private CanvasGroup m_CanvasGroup;

        public Button Button => m_Button;
        public CanvasGroup CanvasGroup => m_CanvasGroup;

        private void Awake() {
            m_Button = GetComponent<Button>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }
    }
}