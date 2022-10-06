using Oculus.Interaction;
using Oculus.Interaction.Input;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ORST.Core.Movement {
    public class TeleportInputHandlerHands : TeleportInputHandler {
        [SerializeField] private Transform m_LeftHand;
        [SerializeField] private Transform m_RightHand;
        [OdinSerialize, Required] private IActiveState m_ActiveState;

        private Hand m_OvrHandLeft;
        private Hand m_OvrHandRight;

        private void Start() {
            m_OvrHandLeft = m_LeftHand.GetComponent<Hand>();
            m_OvrHandRight = m_RightHand.GetComponent<Hand>();
        }

        public override LocomotionTeleport.TeleportIntentions GetIntention() {
            if (!isActiveAndEnabled /*|| !m_ActiveState.Active*/) {
                return LocomotionTeleport.TeleportIntentions.None;
            }

            if (m_OvrHandRight.GetIndexFingerIsPinching() &&
                LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim) {
                return LocomotionTeleport.TeleportIntentions.Teleport;
            }

            if (m_OvrHandLeft.GetIndexFingerIsPinching()) {
                return LocomotionTeleport.TeleportIntentions.Aim;
            }

            return LocomotionTeleport.TeleportIntentions.None;
        }

        public override void GetAimData(out Ray aimRay) {
            Transform pointerPose = LocomotionTeleport.LocomotionController.CameraRig.leftHandAnchor
                                                      .GetComponentInChildren<OVRHand>().PointerPose;
            aimRay = new Ray(LocomotionTeleport.LocomotionController.CameraRig.leftHandAnchor.position,
                             transform.parent.TransformDirection(pointerPose.forward));
        }
    }
}