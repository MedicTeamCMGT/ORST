using DG.Tweening;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.UI {
    public class FollowCamera : BaseMonoBehaviour {
        private const float Y_TRIGGER_DISTANCE = 20;
        private const float X_TRIGGER_DISTANCE = 20;

        [SerializeField, Required] private Transform m_TargetTransform;
        [Space]
        [SerializeField] private bool m_LookAtCamera = true;
        [SerializeField] private bool m_ShouldFollowCamera = true;
        [SerializeField] private bool m_IsFollowing = true;
        [SerializeField] private Vector3 m_PositionOffset = Vector3.zero;
        [SerializeField, SuffixLabel("seconds")] private float m_FollowLatency = 0.5f;
        [SerializeField] private float m_DistanceFromCamera = 2.0f;
        [SerializeField] private AxisConstraint m_FollowAxis = AxisConstraint.X | AxisConstraint.Y | AxisConstraint.Z;

        private Camera m_Camera;
        private Vector3 m_CurrentScreenCenter;
        private bool m_IsInView;
        private RectTransform m_PanelRectTransform;
        private Tween m_TweenMove;

        private void Awake() {
            m_Camera = Camera.main;
            m_PanelRectTransform = GetComponent<RectTransform>();
        }

        private void Start() {
            if (m_DistanceFromCamera > 0.0f) {
                Reposition();
            }
        }

        private void Update() {
            m_CurrentScreenCenter = m_Camera.transform.position + m_Camera.transform.forward * m_DistanceFromCamera;
            float panelHeight = m_PanelRectTransform.rect.height;
            float panelWidth = m_PanelRectTransform.rect.width;

            if (!m_IsFollowing) {
                float distance = Vector3.Distance(m_CurrentScreenCenter, transform.position) / m_PanelRectTransform.localScale.x;

                if (distance > panelWidth + X_TRIGGER_DISTANCE || distance > panelHeight + Y_TRIGGER_DISTANCE) {
                    if (m_ShouldFollowCamera) {
                        UpdateFollowPosition();
                    }

                    if (m_LookAtCamera) {
                        UpdateToLookAtCamera();
                    }
                }
            } else {
                if (m_ShouldFollowCamera) {
                    UpdateFollowPosition();
                }

                if (m_LookAtCamera) {
                    UpdateToLookAtCamera();
                }
            }
        }

        private void Reposition() {
            Transform mainCameraTransform = m_Camera.transform;
            Vector3 targetPosition = mainCameraTransform.position + mainCameraTransform.forward * m_DistanceFromCamera;
            targetPosition.y = m_TargetTransform.position.y;

            Vector3 popupRotation = mainCameraTransform.rotation.eulerAngles;
            popupRotation.z = 0f;
            popupRotation.x = 0f;

            transform.SetPositionAndRotation(targetPosition, Quaternion.Euler(popupRotation));
        }

        private void UpdateToLookAtCamera() {
            Vector3 axisAngle = m_Camera.transform.rotation.eulerAngles;
            transform.DORotate(new Vector3(0f, axisAngle.y, 0f), m_FollowLatency);
        }

        private void UpdateFollowPosition() {
            if ((m_FollowAxis & AxisConstraint.X) != AxisConstraint.X) {
                m_CurrentScreenCenter.x = transform.position.x;
            }

            if ((m_FollowAxis & AxisConstraint.Y) != AxisConstraint.Y) {
                m_CurrentScreenCenter.y = transform.position.y;
            }

            if ((m_FollowAxis & AxisConstraint.Z) != AxisConstraint.Z) {
                m_CurrentScreenCenter.z = transform.position.z;
            }

            if (m_PositionOffset != Vector3.zero && m_IsFollowing) {
                m_CurrentScreenCenter += m_PositionOffset;
            }

            m_TweenMove.Kill();
            m_TweenMove = transform.DOMove(m_CurrentScreenCenter, m_FollowLatency);
        }
    }
}