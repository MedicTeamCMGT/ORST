using Oculus.Interaction.Input;
using ORST.Core.Interactions;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ORST.Core.UI {
    public class DominantHandSetting : BaseMonoBehaviour {
        [SerializeField, Required] private Button m_LeftHandedButton;
        [SerializeField, Required] private Button m_RightHandedButton;

        private void Start() {
            m_LeftHandedButton.onClick.RemoveAllListeners();
            m_RightHandedButton.onClick.RemoveAllListeners();

            m_LeftHandedButton.onClick.AddListener(OnLeftHandedButtonClicked);
            m_RightHandedButton.onClick.AddListener(OnRightHandedButtonClicked);

            HandednessManager.HandednessChanged += OnHandednessChanged;
            OnHandednessChanged(HandednessManager.Handedness);
        }

        private void OnDestroy() {
            HandednessManager.HandednessChanged -= OnHandednessChanged;
        }

        private void OnLeftHandedButtonClicked() {
            HandednessManager.Handedness = Handedness.Left;
        }

        private void OnRightHandedButtonClicked() {
            HandednessManager.Handedness = Handedness.Right;
        }

        private void OnHandednessChanged(Handedness newValue) {
            m_LeftHandedButton.interactable = newValue is not Handedness.Left;
            m_RightHandedButton.interactable = newValue is not Handedness.Right;
        }
    }
}