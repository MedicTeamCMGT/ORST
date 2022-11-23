using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class HandGrabStateProvider : BaseMonoBehaviour, IHandGrabStateProvider {
        [SerializeField] private PassthroughHand m_Hand = PassthroughHand.Dominant;
        [OdinSerialize, Required] private IHandGrabState m_LeftHand;
        [OdinSerialize, Required] private IHandGrabState m_RightHand;

        public IHandGrabState Value => m_Hand switch {
            PassthroughHand.Dominant when HandednessManager.Handedness is Handedness.Left => m_LeftHand,
            PassthroughHand.Dominant when HandednessManager.Handedness is Handedness.Right => m_RightHand,
            PassthroughHand.NonDominant when HandednessManager.Handedness is Handedness.Left => m_RightHand,
            PassthroughHand.NonDominant when HandednessManager.Handedness is Handedness.Right => m_LeftHand,
            PassthroughHand.Left => m_LeftHand,
            PassthroughHand.Right => m_RightHand,
            _ => throw new System.NotImplementedException()
        };

        private enum PassthroughHand {
            Dominant,
            NonDominant,
            Left,
            Right,
        }
    }
}