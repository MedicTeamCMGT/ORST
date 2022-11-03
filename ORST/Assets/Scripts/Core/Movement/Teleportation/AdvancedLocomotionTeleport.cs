using System;
using UnityEngine;

namespace ORST.Core.Movement {
    public class AdvancedLocomotionTeleport : LocomotionTeleport {
        public TeleportTargetHandler TargetHandler { get; set; }
        public TeleportAudioHandler AudioHandler { get; set; }

        public event Action EnteredIntersection;
        public event Action ExitedIntersection;
        public event Action<TeleportPointORST> TeleportedToPoint;

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
            if (TargetHandler is not AdvancedTeleportTargetHandlerNode {TargetPoint: { } targetPoint}) {
                Debug.LogWarning("[Teleportation] Couldn't find target point when teleporting.");
                return;
            }

            TeleportedToPoint?.Invoke(targetPoint);
        }
    }
}