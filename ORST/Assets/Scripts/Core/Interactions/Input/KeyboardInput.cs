using System;
using JetBrains.Annotations;
using ORST.Foundation.Singleton;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class KeyboardInput : AutoMonoSingleton<KeyboardInput> {
        public override bool IsPersistentThroughScenes => true;

        [SerializeField, Required] private TMP_InputField m_InputField;

        private Action<string> m_OnInputChanged;
        private Action m_OnInputFinished;
        private string m_CurrentText;
        private bool m_IsActive;

        protected override void OnAwake() {
            m_InputField.onValueChanged.RemoveAllListeners();
            m_InputField.onValueChanged.AddListener(OnInputChanged);
            m_InputField.onEndEdit.RemoveAllListeners();
            m_InputField.onEndEdit.AddListener(OnInputFinished);
        }

        public void StartInput(string text, [NotNull] Action<string> onInputChanged, Action onInputFinished = null) {
            if (onInputChanged == null) {
                throw new ArgumentNullException(nameof(onInputChanged));
            }

            if (m_IsActive) {
                m_OnInputFinished = null;
                StopInput();
            }

            m_CurrentText = text.Trim();
            m_OnInputChanged = onInputChanged;
            m_OnInputFinished = onInputFinished;

            m_IsActive = true;
            m_InputField.ActivateInputField();
            m_InputField.SetTextWithoutNotify(m_CurrentText);
            m_InputField.caretPosition = m_CurrentText.Length;
        }

        public void StopInput() {
            m_IsActive = false;
            m_InputField.DeactivateInputField();
            m_InputField.SetTextWithoutNotify(string.Empty);
            m_OnInputFinished?.Invoke();

            m_OnInputChanged = null;
            m_OnInputFinished = null;
        }

        private void OnInputChanged(string text) {
            if (!m_IsActive) {
                return;
            }

            text = text.Trim();
            if (string.Equals(m_CurrentText, text, StringComparison.Ordinal)) {
                return;
            }

            m_CurrentText = text;
            m_OnInputChanged?.Invoke(m_CurrentText);
        }

        private void OnInputFinished(string text) {
            if (!m_IsActive) {
                return;
            }

            StopInput();
        }
    }
}