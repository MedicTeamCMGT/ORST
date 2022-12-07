using ORST.Core.Attributes;
using ORST.Foundation;
using ORST.Foundation.Extensions;
using ORST.Foundation.StrongTypes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Attachments {
    public class AttachmentSpace : MonoBehaviour {
        [Title("References")]
        [SerializeField, Required, SdfIcon(SdfIconType.CameraVideoFill)] private OVRCameraRig m_CameraRig;
        [Space]
        [ValidateInput(nameof(ODIN_ValidateAngle), "Angle must be in range [0, 360]")]
        [SerializeField, SuffixLabel("degrees")] private Degrees m_RotationDeadZone = 90.0f;
        [SerializeField] private float m_PositionDeadZone = 0.15f;
        [SerializeField] private float m_MoveSpeed = 1.0f;

        private Vector3 m_TargetPosition;
        private Quaternion m_TargetRotation;
        private bool m_ReachedTarget;

        private void Awake() {
            m_TargetPosition = transform.position;
            m_TargetRotation = transform.rotation;
        }

        private void LateUpdate() {
            UpdateTargetPosition();
            UpdateTargetRotation();

            if (!m_ReachedTarget) {
                MoveTowardsTarget();
            }
        }

        private void MoveTowardsTarget() {
            Transform tr = transform;
            Vector3 position = tr.position = Vector3.Lerp(tr.position, m_TargetPosition, Time.deltaTime * m_MoveSpeed);
            Quaternion rotation = tr.rotation = Quaternion.Lerp(tr.rotation.normalized, m_TargetRotation.normalized, Time.deltaTime * m_MoveSpeed);

            bool reachedPosition = (position - m_TargetPosition).sqrMagnitude < 0.001f;
            bool reachedRotation = Quaternion.Angle(rotation, m_TargetRotation) < 0.001f;
            m_ReachedTarget = reachedPosition && reachedRotation;
        }

        private void UpdateTargetPosition() {
            // check if center eye camera distance on xz plane is greater than dead zone
            Vector3 centerEyePosition = m_CameraRig.centerEyeAnchor.position.WithY(0.0f);
            Vector3 position = transform.position.WithY(0.0f);
            float sqrDistance = (centerEyePosition - position).sqrMagnitude;

            if (sqrDistance > m_PositionDeadZone * m_PositionDeadZone) {
                Vector3 centerEyeLocalPosition = transform.InverseTransformPoint(centerEyePosition);
                m_TargetPosition = transform.TransformPoint(centerEyeLocalPosition.WithY(0.0f));
                m_ReachedTarget = false;
            }
        }

        private void UpdateTargetRotation() {
            // check if angle between camera forward and attachment forward projected on xz plane is greater than dead zone
            Vector3 cameraForward = m_CameraRig.centerEyeAnchor.forward.WithY(0.0f).normalized;
            Vector3 forward = transform.forward.WithY(0.0f).normalized;
            float angle = Vector3.SignedAngle(cameraForward, forward, Vector3.up);

            if (Mathf.Abs(angle) <= m_RotationDeadZone * 0.5f) {
                return;
            }

            Vector3 cameraEuler = m_CameraRig.centerEyeAnchor.eulerAngles;
            m_TargetRotation = Quaternion.Euler(0.0f, cameraEuler.y, 0.0f);
            m_ReachedTarget = false;
        }

        private void OnDrawGizmosSelected() {
            Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
            using var _ = ExtraGizmos.PushMatrix(localToWorldMatrix);

            ExtraGizmos.PushColor(Color.cyan);
            ExtraGizmos.DrawWireDisc(Vector3.zero, Vector3.up, 0.35f);
            Vector3 from = Vector3.forward;
            from = Quaternion.AngleAxis(m_RotationDeadZone * -0.5f, Vector3.up) * from;

            ExtraGizmos.ReplaceColor(Color.cyan.WithA(0.75f));
            ExtraGizmos.DrawSolidArc(Vector3.zero, Vector3.up, from, m_RotationDeadZone, 0.35f);

            ExtraGizmos.ReplaceColor(Color.blue.WithA(0.5f));
            ExtraGizmos.DrawSolidDisc(Vector3.zero, Vector3.up, m_PositionDeadZone);

            ExtraGizmos.PopColor();
        }

        private static bool ODIN_ValidateAngle(Degrees angle) {
            return angle.Value is >= 0.0f and <= 360.0f;
        }
    }
}