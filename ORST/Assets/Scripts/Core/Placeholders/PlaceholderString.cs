using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ORST.Core.Placeholders {
    public class PlaceholderString : BaseMonoBehaviour {
        [SerializeField, Required] private string m_PlaceholderKey;
        [SerializeField] private bool m_UseDefaultValue;
        [SerializeField, ShowIf(nameof(m_UseDefaultValue))]
        private string m_DefaultValue;
        [Space]
        [SerializeField] private UnityEvent<string> m_SetText;

        private void OnEnable() {
            UpdateText();
        }

        private void Start() {
            UpdateText();
        }

        private void UpdateText() {
            if (m_UseDefaultValue) {
                m_SetText.Invoke(PlaceholderManager.GetValueOrDefault(m_PlaceholderKey, m_DefaultValue));
                return;
            }

            if (PlaceholderManager.TryGetValue(m_PlaceholderKey, out string text)) {
                m_SetText.Invoke(text);
            }
        }
    }
}