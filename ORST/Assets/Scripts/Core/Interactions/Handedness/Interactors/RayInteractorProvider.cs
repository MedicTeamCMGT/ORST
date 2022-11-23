using Oculus.Interaction;
using Oculus.Interaction.Input;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class RayInteractorProvider : BaseMonoBehaviour, IRayInteractorProvider {
        [SerializeField] private PassthroughHand m_Hand = PassthroughHand.Dominant;
        [SerializeField, Required] private RayInteractor m_LeftHand;
        [SerializeField, Required] private RayInteractor m_RightHand;

        public RayInteractor Value => m_Hand switch {
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