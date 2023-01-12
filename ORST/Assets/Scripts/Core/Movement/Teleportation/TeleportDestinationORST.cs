using UnityEngine;

namespace ORST.Core.Movement {
    public class TeleportDestinationORST : TeleportDestination {
        [SerializeField] private bool m_UseTeleportPointRotation = true;

        public override void UpdateTeleportDestination(bool isValidDestination, Vector3? position, Quaternion? rotation, Quaternion? landingRotation) {
            if (m_UseTeleportPointRotation) {
                if (LocomotionTeleport is AdvancedLocomotionTeleport { TargetHandler: AdvancedTeleportTargetHandlerNode {TargetPoint: var targetPoint } } && targetPoint != null) {
                    if (targetPoint == null) {
                        Debug.LogError("Teleport target point is null");
                    }

                    if (targetPoint.DestinationTransform == null) {
                        Debug.LogError("Teleport target point destination transform is null");
                    }

                    Debug.Log("Updated teleport destination rotation to teleport point rotation");
                    landingRotation = targetPoint.DestinationTransform.rotation;
                }
            }

            base.UpdateTeleportDestination(isValidDestination, position, rotation, landingRotation);
        }
    }
}