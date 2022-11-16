using DG.Tweening;
using Oculus.Interaction;
using Oculus.Interaction.PoseDetection;
using ORST.Core.Interactions;
using ORST.Foundation.Extensions;
using Sirenix.Serialization;
using UnityEngine;
using Tween = DG.Tweening.Tween;

namespace ORST.Core.Movement {
    public class TeleportInputHandlerHands : TeleportInputHandler {
        [OdinSerialize] private IActiveState m_ActiveState;
        [SerializeField] private ShapeRecognizerActiveState m_AimRecognizer;
        private bool m_TeleportTested;
        private readonly float m_AimThreshold = 0.1f;
        private Tween m_HoldAimIntention;
        private LocomotionTeleport.TeleportIntentions m_CurrentIntention;

        public override LocomotionTeleport.TeleportIntentions GetIntention() {
            if (!isActiveAndEnabled || m_ActiveState.OrNull() is { Active: false }) {
                StopHoldAimIntention();
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
                return m_CurrentIntention;
            }

            if (m_CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && HandednessManager.NonDominantHand.GetIndexFingerIsPinching()) {
                StopHoldAimIntention();
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.Teleport;
                return m_CurrentIntention;
            }

            if (m_AimRecognizer.Active) {
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.Aim;
                StopHoldAimIntention();
                return m_CurrentIntention;
            }

            if (m_CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim && m_HoldAimIntention == null) {
                m_HoldAimIntention = DOVirtual.DelayedCall(m_AimThreshold, () => {
                    m_CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
                });
            }

            return m_CurrentIntention;
        }

        private void StopHoldAimIntention() {
            if (m_HoldAimIntention is { active: true }) {
                m_HoldAimIntention.Kill();
            }

            m_HoldAimIntention = null;
        }

        public override void GetAimData(out Ray aimRay) {
            HandednessManager.NonDominantHand.GetPointerPose(out Pose pose);
            aimRay = new Ray(pose.position, pose.forward);
        }
    }
}
