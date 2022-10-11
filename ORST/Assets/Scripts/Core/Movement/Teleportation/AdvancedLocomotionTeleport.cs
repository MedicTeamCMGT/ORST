using System;

namespace ORST.Core.Movement {
    public class AdvancedLocomotionTeleport : LocomotionTeleport {
        public TeleportTargetHandler TargetHandler { get; set; }
        public TeleportAudioHandler AudioHandler { get; set; }

        public event Action OnIntersectEnter;
        public event Action OnIntersectExit;

        public void InvokeOnIntersectEnter() {
            OnIntersectEnter?.Invoke();
        }

        public void InvokeOnIntersectExit() {
            OnIntersectExit?.Invoke();
        }
    }
}