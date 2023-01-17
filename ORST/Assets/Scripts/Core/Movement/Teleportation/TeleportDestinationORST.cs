using UnityEngine;

namespace ORST.Core.Movement {
    public class TeleportDestinationORST : TeleportDestination {
        [SerializeField] private bool m_UseTeleportPointRotation = true;

        public override void UpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation) {
            if (m_UseTeleportPointRotation) {
                if (LocomotionTeleport is AdvancedLocomotionTeleport { TargetHandler: AdvancedTeleportTargetHandlerNode {TargetPoint: var targetPoint } } && targetPoint != null) {
                    if (targetPoint == null || targetPoint.DestinationTransform == null) {
                        return;
                    }

                    landingRotation = targetPoint.DestinationTransform.rotation;
                }
            }

            base.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
        }
    }
}