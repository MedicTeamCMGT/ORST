using ORST.Core.UI;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class ForbiddenZonePopupHandler : BaseMonoBehaviour {
        [SerializeField, Required] private ForbiddenZone m_ForbiddenZone;
        [Space]
        [SerializeField] private string m_Title;
        [SerializeField] private string m_Message;

        private void OnEnable() {
            m_ForbiddenZone.EnteredZone += OnEnteredZone;
            m_ForbiddenZone.ExitedZone += OnExitedZone;
        }

        private void OnDisable() {
            m_ForbiddenZone.EnteredZone -= OnEnteredZone;
            m_ForbiddenZone.ExitedZone -= OnExitedZone;
        }

        private void OnEnteredZone() {
            if (!PopupManager.Instance.IsPopupShown()) {
                PopupManager.Instance.OpenPopup();
                PopupManager.Instance.DisplayInfo(m_Title, m_Message);
            }
        }

        private void OnExitedZone() {
            PopupManager.Instance.ClosePopup();
        }
    }
}