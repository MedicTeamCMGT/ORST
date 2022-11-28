using Oculus.Interaction;
using Oculus.Interaction.Input;
using ORST.Core.Interactions;
using ORST.Foundation.Extensions;
using UnityEngine;

namespace ORST.Core.Movement {
    public class TeleportInputHandlerHands : TeleportInputHandler {
        [SerializeField, Interface(typeof(IActiveState))]
        private MonoBehaviour m_ActiveState;

        private LocomotionTeleport.TeleportIntentions m_CurrentIntention;
        private bool m_ExecutedTeleport;

        private IActiveState ActiveState => m_ActiveState as IActiveState;

        public override LocomotionTeleport.TeleportIntentions GetIntention() {
            if (!isActiveAndEnabled || ActiveState.OrNull() is { Active: false }) {
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
                return m_CurrentIntention;
            }

            if (m_CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim
             && HandednessManager.NonDominantHand.GetIndexFingerIsPinching() && !m_ExecutedTeleport) {
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.Teleport;
                m_ExecutedTeleport = true;
                return m_CurrentIntention;
            }

            if (HandednessManager.NonDominantHand.GetFingerPinchStrength(HandFinger.Index) < 0.5f) {
                m_ExecutedTeleport = false;
            }

            if (m_ExecutedTeleport) {
                return m_CurrentIntention;
            }

            if (HandednessManager.NonDominantHand.IsPointerPoseValid) {
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
                return m_CurrentIntention;
            }

            m_CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
            return m_CurrentIntention;
        }

        public override void GetAimData(out Ray aimRay) {
            HandednessManager.NonDominantHand.GetPointerPose(out Pose pose);
            aimRay = new Ray(pose.position, pose.forward);
        }
    }
}