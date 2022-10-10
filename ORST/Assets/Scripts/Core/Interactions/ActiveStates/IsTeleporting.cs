using Oculus.Interaction;
using ORST.Core.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class IsTeleporting : MonoBehaviour, IActiveState {
        [SerializeField, Required] private TeleportInputHandlerHands m_TeleportInputHandler;

        public bool Active => m_TeleportInputHandler.GetIntention() != LocomotionTeleport.TeleportIntentions.None;
    }
}