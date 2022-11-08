using Oculus.Interaction.HandGrab;
using Oculus.Interaction.HandGrab.Visuals;
using Oculus.Interaction.Input;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class HandPoseData : SerializedScriptableObject {
        [OdinSerialize, ReadOnly] private HandPose m_HandPose;
        [OdinSerialize, ReadOnly] private Vector3[] m_JointPositions;

        public HandPose HandPose => m_HandPose;

        public Pose? this[HandJointId jointId] {
            get {
                int offset = (int)FingersMetadata.HAND_JOINT_IDS[0];
                int index = (int)jointId - offset;
                if (index < 0 || index >= m_JointPositions.Length) {
                    return null;
                }

                return new Pose(m_JointPositions[index], m_HandPose.JointRotations[index]);
            }
        }

        public void InitializeFromJointCollection(JointCollection jointCollection, Handedness handedness) {
            m_HandPose = new HandPose(handedness);
            m_JointPositions = new Vector3[FingersMetadata.HAND_JOINT_IDS.Length];
            for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++) {
                m_HandPose.JointRotations[i] = jointCollection[i]?.TrackedRotation ?? Quaternion.identity;
                m_JointPositions[i] = jointCollection[i]?.transform.localPosition ?? Vector3.zero;
            }
        }
    }
}