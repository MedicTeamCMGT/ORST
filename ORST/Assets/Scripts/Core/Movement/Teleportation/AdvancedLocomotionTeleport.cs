using System;
using UnityEngine;

namespace ORST.Core.Movement {
    public class AdvancedLocomotionTeleport : LocomotionTeleport {
        public TeleportTargetHandler TargetHandler { get; set; }
        public TeleportAudioHandler AudioHandler { get; set; }

        public event Action EnteredIntersection;
        public event Action ExitedIntersection;
        public event Action<TeleportPoint> TeleportedToPoint;

        /// <summary>
        /// Start the state machine coroutines.
        /// </summary>
        public override void OnEnable() {
            base.OnEnable();

            Teleported -= OnTeleported;
            Teleported += OnTeleported;
        }

        public override void OnDisable() {
            base.OnDisable();

            Teleported -= OnTeleported;
        }

        public void InvokeOnIntersectEnter() {
            EnteredIntersection?.Invoke();
        }

        public void InvokeOnIntersectExit() {
            ExitedIntersection?.Invoke();
        }

        private void OnTeleported(Transform controllerTransform, Vector3 position, Quaternion rotation) {
            if (TargetHandler is not AdvancedTeleportTargetHandlerNode nodeHandler) {
                return;
            }

            if (nodeHandler.AimData.TargetHitInfo.collider == null ||
                nodeHandler.AimData.TargetHitInfo.collider.GetComponent<TeleportPoint>() is not {} teleportPoint) {
                return;
            }

            TeleportedToPoint?.Invoke(teleportPoint);
        }
    }
}