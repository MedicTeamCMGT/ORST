using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Movement {
    [DefaultExecutionOrder(-10)]
    public class AdvancedLocomotionTeleport : LocomotionTeleport {
        public static AdvancedLocomotionTeleport Instance { get; private set; }

        public TeleportTargetHandler TargetHandler { get; set; }
        public TeleportAudioHandler AudioHandler { get; set; }

        public event Action EnteredIntersection;
        public event Action ExitedIntersection;
        public event Action<TeleportPointORST> TeleportedToPoint;

        /// <summary>
        /// When the component first wakes up, cache the LocomotionController and the initial
        /// TeleportDestination object.
        /// </summary>
        protected override void Awake() {
            base.Awake();

            Instance = this;
        }

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

        [Button]
        public void Teleport(TeleportPointORST teleportPoint) {
            Vector3 destinationPosition = teleportPoint.DestinationTransform.position;
            destinationPosition.y += LocomotionController.CharacterController.height * 0.5f;

            Quaternion destinationRotation = teleportPoint.DestinationTransform.rotation;

            Transform characterTransform = LocomotionController.CharacterController.transform;
            characterTransform.position = destinationPosition;
            characterTransform.rotation = destinationRotation;

            TeleportedToPoint?.Invoke(teleportPoint);
        }

        private void OnTeleported(Transform controllerTransform, Vector3 position, Quaternion rotation) {
            if (TargetHandler is not AdvancedTeleportTargetHandlerNode {TargetPoint: { } targetPoint}) {
                return;
            }

            TeleportedToPoint?.Invoke(targetPoint);
        }
    }
}