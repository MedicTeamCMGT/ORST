using ORST.Foundation.Singleton;
using UnityEngine;

namespace ORST.Core.Movement {
    [DefaultExecutionOrder(-10)]
    public class DisableCurrentTeleportPoint : AutoMonoSingleton<DisableCurrentTeleportPoint> {
        public override bool IsPersistentThroughScenes => true;
        public bool IsDisabled { get; set; }

        private TeleportPointORST m_CurrentTeleportPoint;

        private void OnEnable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint += OnTeleportedToPoint;
        }

        private void OnDisable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint -= OnTeleportedToPoint;
        }

        private void OnTeleportedToPoint(TeleportPointORST teleportPoint) {
            if (IsDisabled) {
                return;
            }

            if (m_CurrentTeleportPoint != null) {
                TeleportPointManager.EnablePoint(m_CurrentTeleportPoint);
            }

            m_CurrentTeleportPoint = teleportPoint;
            TeleportPointManager.DisablePoint(m_CurrentTeleportPoint);
        }
    }
}