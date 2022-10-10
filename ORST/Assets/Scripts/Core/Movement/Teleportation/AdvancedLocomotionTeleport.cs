using System;

namespace ORST.Core.Movement {
    public class AdvancedLocomotionTeleport : LocomotionTeleport {
        public TeleportTargetHandler TargetHandler { get; set; }
        public TeleportAudioHandler AudioHandler { get; set; }

        public event Action EnteredIntersection;
        public event Action ExitedIntersection;

        public void InvokeOnIntersectEnter() {
            EnteredIntersection?.Invoke();
        }

        public void InvokeOnIntersectExit() {
            ExitedIntersection?.Invoke();
        }
    }
}