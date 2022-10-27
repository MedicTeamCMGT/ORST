using System;
using Oculus.Interaction.Input;
using ORST.Foundation.Core;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class NonDominantHand : BaseMonoBehaviour, IHand {
        private IHand m_Hand;

        private void Awake() {
            HandednessManager.HandednessChanged += OnHandednessChanged;
            m_Hand = HandednessManager.NonDominantHand;
        }

        private void OnDestroy() {
            HandednessManager.HandednessChanged -= OnHandednessChanged;
        }

        private void OnHandednessChanged(Handedness newHandedness) {
            m_Hand = HandednessManager.NonDominantHand;
        }

        #region IHand

        public Handedness Handedness => m_Hand.Handedness;

        public bool IsConnected => m_Hand.IsConnected;

        /// <summary>
        /// The hand is connected and tracked, and the root pose's tracking data is marked as
        /// high confidence.
        /// If this is true, then it implies that IsConnected and IsRootPoseValid are also true,
        /// so they don't need to be checked in addition to this.
        /// </summary>
        public bool IsHighConfidence => m_Hand.IsHighConfidence;

        public bool IsDominantHand => m_Hand.IsDominantHand;

        public float Scale => m_Hand.Scale;

        public bool GetFingerIsPinching(HandFinger finger) {
            return m_Hand.GetFingerIsPinching(finger);
        }

        public bool GetIndexFingerIsPinching() {
            return m_Hand.GetIndexFingerIsPinching();
        }

        /// <summary>
        /// Will return true if a pointer pose is available, that can be retrieved via
        /// <see cref="IHand.GetPointerPose"/>
        /// </summary>
        public bool IsPointerPoseValid => m_Hand.IsPointerPoseValid;

        /// <summary>
        /// Attempts to calculate the pose that can be used as a root for raycasting, in world space
        /// Returns false if there is no valid tracking data.
        /// </summary>
        public bool GetPointerPose(out Pose pose) {
            return m_Hand.GetPointerPose(out pose);
        }

        /// <summary>
        /// Attempts to calculate the pose of the requested hand joint, in world space.
        /// Returns false if the skeleton is not yet initialized, or there is no valid
        /// tracking data.
        /// </summary>
        public bool GetJointPose(HandJointId handJointId, out Pose pose) {
            return m_Hand.GetJointPose(handJointId, out pose);
        }

        /// <summary>
        /// Attempts to calculate the pose of the requested hand joint, in local space.
        /// Returns false if the skeleton is not yet initialized, or there is no valid
        /// tracking data.
        /// </summary>
        public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose) {
            return m_Hand.GetJointPoseLocal(handJointId, out pose);
        }

        /// <summary>
        /// Returns an array containing the local pose of each joint. The poses
        /// do not have the root pose applied, nor the hand scale. It is in the same coordinate
        /// system as the hand skeleton.
        /// </summary>
        /// <param name="localJointPoses">The array with the local joint poses.
        /// It will be empty if no poses where found</param>
        /// <returns>
        /// True if the poses collection was correctly populated. False otherwise.
        /// </returns>
        public bool GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses) {
            return m_Hand.GetJointPosesLocal(out localJointPoses);
        }

        /// <summary>
        /// Attempts to calculate the pose of the requested hand joint relative to the wrist.
        /// Returns false if the skeleton is not yet initialized, or there is no valid
        /// tracking data.
        /// </summary>
        public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose) {
            return m_Hand.GetJointPoseFromWrist(handJointId, out pose);
        }

        /// <summary>
        /// Returns an array containing the pose of each joint relative to the wrist. The poses
        /// do not have the root pose applied, nor the hand scale. It is in the same coordinate
        /// system as the hand skeleton.
        /// </summary>
        /// <param name="jointPosesFromWrist">The array with the joint poses from the wrist.
        /// It will be empty if no poses where found</param>
        /// <returns>
        /// True if the poses collection was correctly populated. False otherwise.
        /// </returns>
        public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist) {
            return m_Hand.GetJointPosesFromWrist(out jointPosesFromWrist);
        }

        /// <summary>
        /// Obtains palm pose in local space.
        /// </summary>
        /// <param name="pose">The pose to populate</param>
        /// <returns>
        /// True if pose was obtained.
        /// </returns>
        public bool GetPalmPoseLocal(out Pose pose) {
            return m_Hand.GetPalmPoseLocal(out pose);
        }

        public bool GetFingerIsHighConfidence(HandFinger finger) {
            return m_Hand.GetFingerIsHighConfidence(finger);
        }

        public float GetFingerPinchStrength(HandFinger finger) {
            return m_Hand.GetFingerPinchStrength(finger);
        }

        /// <summary>
        /// True if the hand is currently tracked, thus tracking poses are available for the hand
        /// root and finger joints.
        /// This property does not indicate pointing pose validity, which has its own property:
        /// <see cref="IHand.IsPointerPoseValid"/>.
        /// </summary>
        public bool IsTrackedDataValid => m_Hand.IsTrackedDataValid;

        /// <summary>
        /// Gets the root pose of the wrist, in world space.
        /// Will return true if a pose was available; false otherwise.
        /// Confidence level of the pose is exposed via <see cref="IHand.IsHighConfidence"/>.
        /// </summary>
        public bool GetRootPose(out Pose pose) {
            return m_Hand.GetRootPose(out pose);
        }

        /// <summary>
        /// Will return true if an HMD Center Eye pose available, that can be retrieved via
        /// <see cref="IHand.GetCenterEyePose"/>
        /// </summary>
        public bool IsCenterEyePoseValid => m_Hand.IsCenterEyePoseValid;

        /// <summary>
        /// Gets the pose of the center eye (HMD), in world space.
        /// Will return true if a pose was available; false otherwise.
        /// </summary>
        public bool GetCenterEyePose(out Pose pose) {
            return m_Hand.GetCenterEyePose(out pose);
        }

        /// <summary>
        /// The transform that was applied to all tracking space poses to convert them to world
        /// space.
        /// </summary>
        public Transform TrackingToWorldSpace => m_Hand.TrackingToWorldSpace;
        /// <summary>
        /// Incremented every time the source tracking or state data changes.
        /// </summary>
        public int CurrentDataVersion => m_Hand.CurrentDataVersion;

        /// <summary>
        /// An Aspect provides additional functionality on top of what the HandState provides.
        /// The underlying hand is responsible for finding the most appropriate component.
        /// It is usually, but not necessarily, located within the same GameObject as the
        /// underlying hand.
        /// For example, this method can be used to source the SkinnedMeshRenderer representing the
        /// hand, if one exists.
        /// <returns>true if an aspect of the requested type was found, false otherwise</returns>
        /// </summary>
        public bool GetHandAspect<TComponent>(out TComponent foundComponent) where TComponent : class {
            return m_Hand.GetHandAspect(out foundComponent);
        }

        public event Action WhenHandUpdated {
            add => m_Hand.WhenHandUpdated += value;
            remove => m_Hand.WhenHandUpdated -= value;
        }

        #endregion
    }
}