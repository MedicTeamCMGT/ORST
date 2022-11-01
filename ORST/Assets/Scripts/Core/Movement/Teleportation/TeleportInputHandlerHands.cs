using DG.Tweening;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using Oculus.Interaction.PoseDetection;
using ORST.Foundation.Extensions;
using Sirenix.Serialization;
using UnityEngine;
using Tween = DG.Tweening.Tween;

namespace ORST.Core.Movement {
    public class TeleportInputHandlerHands : TeleportInputHandler {
        [SerializeField] private Hand m_LeftHand;
        [SerializeField] private Hand m_RightHand;
        [OdinSerialize] private IActiveState m_ActiveState;
        [SerializeField] private ShapeRecognizerActiveState m_ShapeRecognizerAim;
        [SerializeField] private ShapeRecognizerActiveState m_ShapeRecognizerTeleport;

        private readonly float m_AimThreshold = 0.1f;
        private Tween m_HoldAimIntention;
        private LocomotionTeleport.TeleportIntentions m_CurrentIntention;

        public override LocomotionTeleport.TeleportIntentions GetIntention() {
            if (!isActiveAndEnabled || m_ActiveState.OrNull() is { Active: false }) {
                StopHoldAimIntention();
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
                return m_CurrentIntention;
            }

            if (m_ShapeRecognizerTeleport.Active &&
                m_CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim) {
                StopHoldAimIntention();
                m_CurrentIntention = LocomotionTeleport.TeleportIntentions.Teleport;
                return m_CurrentIntention;
            }

            if (m_ShapeRecognizerAim.Active) {
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
            m_LeftHand.GetJointPose(HandJointId.HandIndex2, out Pose pose);
            aimRay = new Ray(pose.position - pose.right * 0.05f, -pose.right);
        }
    }
}
