using System;

namespace ORST.Core
{
    public class AdvancedLocomotionTeleport : LocomotionTeleport {
        [field: NonSerialized] public TeleportTargetHandler TargetHandler { get; set; }

        [field: NonSerialized] public TeleportAudioHandler AudioHandler { get; set; }

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
