using System;
using ORST.Foundation;
using ORST.Foundation.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.LearningModules {
    public class NameTagDropZone : BaseMonoBehaviour {
        public event Action<NameTagKind> NameTagChanged;

        [Title("References")]
        [SerializeField, Required] private Collider m_DropZoneCollider;

        [Title("Animation Settings")]
        [SerializeField] private float m_GoToCenterAnimationDuration = 1.0f;
        [SerializeField] private float m_Rotate360AnimationSpeed = 0.125f;
        [SerializeField] private float m_BobbleAnimationSpeed = 0.25f;
        [SerializeField] private float m_BobbleAmount = 0.025f;
        [SerializeField] private Vector3 m_RotateOffsetAngle = new(-90.0f, 0.0f, 0.0f);

        private NameTagObject m_NameTagObject;
        private NameTagKind m_NameTagKind;

        private DropZoneAnimationState m_AnimationState;
        private DropZoneAnimationState m_LastAnimationState;
        private float m_EnterStateTime;

        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;

        private void InitializeNameTag() {
            // This probably means that it's not being grabbed so we can go ahead and start the animation
            if (m_NameTagObject.Grabbable.SelectingPoints?.Count == 0) {
                m_NameTagObject.Rigidbody.isKinematic = true;
                StartAnimation();
                return;
            }

            m_NameTagObject.PointableEventWrapper.WhenSelect.AddListener(OnNameTagPickedUp);
            m_NameTagObject.PointableEventWrapper.WhenUnselect.AddListener(OnNameTagReleased);
        }

        private void ResetNameTag() {
            m_NameTagObject.PointableEventWrapper.WhenSelect.RemoveListener(OnNameTagPickedUp);
            m_NameTagObject.PointableEventWrapper.WhenUnselect.RemoveListener(OnNameTagReleased);
            StopAnimation();
        }

        private void OnNameTagPickedUp() {
            StopAnimation();
        }

        private void OnNameTagReleased() {
            m_NameTagObject.Rigidbody.isKinematic = true;
            StartAnimation();
        }

        private void StartAnimation() {
            m_OriginalPosition = m_NameTagObject.transform.position;
            m_OriginalRotation = m_NameTagObject.transform.rotation;
            m_AnimationState = DropZoneAnimationState.MoveTowardsDefaultPosition;
            m_LastAnimationState = DropZoneAnimationState.None;
        }

        private void StopAnimation() {
            m_AnimationState = DropZoneAnimationState.None;
        }

        private void Update() {
            if (m_AnimationState == DropZoneAnimationState.None || m_NameTagObject == null) {
                return;
            }

            if (m_LastAnimationState != m_AnimationState) {
                m_EnterStateTime = Time.time;
                m_LastAnimationState = m_AnimationState;
            }

            switch (m_AnimationState) {
                case DropZoneAnimationState.MoveTowardsDefaultPosition:
                    MoveTowardsDefaultPositionAnimation();
                    break;
                case DropZoneAnimationState.ContinuouslyRotateAndBobble:
                    ContinuouslyRotateAndBobbleAnimation();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveTowardsDefaultPositionAnimation() {
            float progress = (Time.time - m_EnterStateTime) / m_GoToCenterAnimationDuration;

            Vector3 targetPosition = m_DropZoneCollider.bounds.center;
            Quaternion targetRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

            m_NameTagObject.Rigidbody.position = Vector3.Lerp(m_OriginalPosition, targetPosition, progress);
            m_NameTagObject.Rigidbody.rotation = Quaternion.Lerp(m_OriginalRotation, targetRotation, progress);

            if (progress >= 1.0f) {
                m_AnimationState = DropZoneAnimationState.ContinuouslyRotateAndBobble;

                m_OriginalPosition = m_NameTagObject.transform.position;
                m_OriginalRotation = m_NameTagObject.transform.rotation;
            }
        }

        private void ContinuouslyRotateAndBobbleAnimation() {
            float angle = Time.time * m_Rotate360AnimationSpeed * 360.0f;
            m_NameTagObject.Rigidbody.rotation = Quaternion.Euler(m_RotateOffsetAngle.x, m_RotateOffsetAngle.y + angle, m_RotateOffsetAngle.z);

            float bobbleAmount = Mathf.Sin(Time.time * m_BobbleAnimationSpeed) * m_BobbleAmount;
            float y = m_DropZoneCollider.bounds.center.y + bobbleAmount;

            Vector3 rigidbodyPosition = m_NameTagObject.Rigidbody.position;
            m_NameTagObject.Rigidbody.position = new Vector3(rigidbodyPosition.x, y, rigidbodyPosition.z);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<NameTagObject>() is not { } nameTagObject) return;

            if (m_NameTagObject != null) {
                ResetNameTag();
                m_NameTagObject.ResetPosition();
                m_NameTagObject.Rigidbody.isKinematic = false;
                m_NameTagObject = null;
            }

            m_NameTagObject = nameTagObject;
            m_NameTagKind = nameTagObject.Kind;
            NameTagChanged?.Invoke(m_NameTagKind);
            InitializeNameTag();
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject != m_NameTagObject.OrNull()?.gameObject) {
                if (other.GetComponent<NameTagObject>() is { } nameTagObject) {
                    nameTagObject.PointableEventWrapper.WhenSelect.RemoveListener(OnNameTagPickedUp);
                    nameTagObject.PointableEventWrapper.WhenUnselect.RemoveListener(OnNameTagReleased);
                }

                return;
            }

            ResetNameTag();
            m_NameTagObject = null;
            m_NameTagKind = NameTagKind.None;
            NameTagChanged?.Invoke(m_NameTagKind);
        }

        private enum DropZoneAnimationState {
            None,
            MoveTowardsDefaultPosition,
            ContinuouslyRotateAndBobble,
        }
    }
}