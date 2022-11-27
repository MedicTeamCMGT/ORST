using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace ORST.Core.Movement {
    public class TeleportOnStart : BaseMonoBehaviour {
        [SerializeField, Required] private AdvancedLocomotionTeleport m_AdvancedLocomotionTeleport;
        [SerializeField, Required] private TeleportPointORST m_TeleportPoint;

        private void Start() {
            Assert.IsNotNull(m_AdvancedLocomotionTeleport, "AdvancedLocomotionTeleport is null");
            Assert.IsNotNull(m_TeleportPoint, "TeleportPoint is null");

            m_AdvancedLocomotionTeleport.Teleport(m_TeleportPoint);
        }
    }
}