using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace ORST.Core.UI {
    public sealed class FollowCamera : MonoBehaviour {
        private const float Y_TRIGGER_DISTANCE = 20;
        private const float X_TRIGGER_DISTANCE = 20;

        [SerializeField] private bool lookAtCamera = true;
        [SerializeField] private bool shouldFollowCamera = true;
        [SerializeField] private bool isFollowing;
        [SerializeField] private Vector3 positionOffeset = new Vector3(0, 0, 0);
        [SerializeField] private float followLatency = 0.5f;
        [SerializeField] private float distanceFromCamera = float.NegativeInfinity;
        [SerializeField] private AxisConstraint followAxis;

        private Camera m_Camera;

        private Vector3 m_CurrentScreenCenter;

        private bool m_IsInView;
        private float m_PanelHeight;
        private RectTransform m_PanelRectTransform;

        private float m_PanelWidth;

        private Tween m_TweenMove;

        [FormerlySerializedAs("vrPlayer")]
        [SerializeField] private Transform m_TargetTransform;


        /// <summary>
        ///     Reposition the component.
        /// </summary>
        public void Reposition() {
            Camera mainCamera = Camera.main;

            Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;
            targetPosition.y = m_TargetTransform.position.y;

            Vector3 popupRotation = mainCamera.transform.rotation.eulerAngles;
            popupRotation.z = 0f;
            popupRotation.x = 0f;

            transform.SetPositionAndRotation(targetPosition, Quaternion.Euler(popupRotation));
        }


        protected void Start() {
            m_Camera = Camera.main;

            if (distanceFromCamera > 0) Reposition();

            m_PanelRectTransform = GetComponent<RectTransform>();
        }

        protected void Update() {
            m_CurrentScreenCenter = m_Camera.transform.position + m_Camera.transform.forward * distanceFromCamera;
            m_PanelHeight = m_PanelRectTransform.rect.height;
            m_PanelWidth = m_PanelRectTransform.rect.width;

            if (!isFollowing) {
                float distance = Mathf.Sqrt(Mathf.Pow(m_CurrentScreenCenter.x - transform.position.x, 2) +
                                            Mathf.Pow(m_CurrentScreenCenter.y - transform.position.y, 2) +
                                            Mathf.Pow(m_CurrentScreenCenter.z - transform.position.z, 2));

                distance /= m_PanelRectTransform.localScale.x;

                if (distance > m_PanelWidth + X_TRIGGER_DISTANCE || distance > m_PanelHeight + Y_TRIGGER_DISTANCE) {
                    if (shouldFollowCamera) UpdateFollowPossition();

                    if (lookAtCamera) UpdateToLookAtCamera();
                }
            } else {
                if (shouldFollowCamera) UpdateFollowPossition();

                if (lookAtCamera) UpdateToLookAtCamera();
            }
        }


        private void UpdateToLookAtCamera() {
            Vector3 axisAngle = m_Camera.transform.rotation.eulerAngles;
            transform.DORotate(new Vector3(0f, axisAngle.y, 0f), followLatency);
        }

        private void UpdateFollowPossition() {
            if ((followAxis & AxisConstraint.X) != AxisConstraint.X) m_CurrentScreenCenter.x = transform.position.x;

            if ((followAxis & AxisConstraint.Y) != AxisConstraint.Y) m_CurrentScreenCenter.y = transform.position.y;

            if ((followAxis & AxisConstraint.Z) != AxisConstraint.Z) m_CurrentScreenCenter.z = transform.position.z;

            if (positionOffeset != Vector3.zero && isFollowing) m_CurrentScreenCenter += positionOffeset;

            m_TweenMove.Kill();
            m_TweenMove = transform.DOMove(m_CurrentScreenCenter, followLatency);
        }

    }
}