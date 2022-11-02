using DG.Tweening;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class TestKeyboardInput : BaseMonoBehaviour {
        [SerializeField, SuffixLabel("seconds")] private float m_AnimationDuration = 0.5f;
        [SerializeField] private float m_TitleOffset = 64.0f;
        [Space]
        [SerializeField, Required] private TextMeshProUGUI m_TitleLabel;
        [SerializeField, Required] private TextMeshProUGUI m_InputLabel;

        private bool m_IsInputShown;

        [Button]
        public void StartInput() {
            m_InputLabel.text = string.Empty;
            m_TitleLabel.rectTransform.DOKill();
            m_TitleLabel.rectTransform.DOAnchorPosY(0.0f, m_AnimationDuration);
            m_IsInputShown = false;

            KeyboardInput.Instance.StartInput(string.Empty, OnInputChanged, () => {
                Debug.Log("Input finished");
            });
        }

        [Button]
        private void StartInput(string text) {
            KeyboardInput.Instance.StartInput(text, OnInputChanged, () => {
                Debug.Log("Input finished");
            });
        }

        [Button]
        private void StopInput() {
            KeyboardInput.Instance.StopInput();
        }

        private void OnInputChanged(string text) {
            m_InputLabel.text = text.Trim();

            bool isEmpty = string.IsNullOrWhiteSpace(m_InputLabel.text);
            if (isEmpty && m_IsInputShown) {
                m_TitleLabel.rectTransform.DOKill();
                m_TitleLabel.rectTransform.DOAnchorPosY(0.0f, m_AnimationDuration);
                m_IsInputShown = false;
            } else if (!isEmpty && !m_IsInputShown) {
                m_TitleLabel.rectTransform.DOKill();
                m_TitleLabel.rectTransform.DOAnchorPosY(m_TitleOffset, m_AnimationDuration);
                m_IsInputShown = true;
            }
        }
    }
}