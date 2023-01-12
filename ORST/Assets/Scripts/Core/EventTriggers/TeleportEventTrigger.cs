using ORST.Core.Movement;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ORST.Core.EventTriggers {
    public class TeleportEventTrigger : BaseMonoBehaviour {
        [SerializeField, Required] private TeleportPointORST m_TeleportPoint;
        [SerializeField] private UnityEvent m_OnTeleport;
        [SerializeField] private UnityEvent m_OnLeaveTeleportPoint;

        private bool m_IsInTeleportPoint;

        private void OnEnable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint += OnTeleportedToPoint;
        }

        private void OnDisable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint -= OnTeleportedToPoint;
        }

        private void OnTeleportedToPoint(TeleportPointORST teleportPoint) {
            if (teleportPoint == m_TeleportPoint) {
                m_OnTeleport.Invoke();
                m_IsInTeleportPoint = true;
            } else if (m_IsInTeleportPoint) {
                m_OnLeaveTeleportPoint.Invoke();
                m_IsInTeleportPoint = false;
            }
        }
    }
}