using System;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.HandGrab.Visuals;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    [ExecuteAlways]
    public class HandPoseInterpolatorVisualizer : BaseMonoBehaviour {
        [SerializeField, Required] private HandPoseData m_HandPoseA;
        [SerializeField, Required] private HandPoseData m_HandPoseB;
        [SerializeField, Required] private HandPuppet m_Puppet;
        [Space]
        [SerializeField, Range(0.0f, 1.0f), OnValueChanged(nameof(OnTChanged))] private float m_T;

        private HandPose m_InterpolatedPose;

        private void Update() {
            Interpolate();
        }

        private void Interpolate() {
            HandPoseInterpolator.Interpolate(m_HandPoseA, m_HandPoseB, m_InterpolatedPose ??= new HandPose(m_HandPoseA.HandPose), m_T);
            m_Puppet.SetJointRotations(m_InterpolatedPose.JointRotations);
        }

        private void OnTChanged() {
            if (m_HandPoseA == null || m_HandPoseB == null || m_Puppet == null) {
                return;
            }

            Interpolate();
        }
    }
}