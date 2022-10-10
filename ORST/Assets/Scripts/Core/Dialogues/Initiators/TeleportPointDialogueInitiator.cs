using System;
using ORST.Core.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    public class TeleportPointDialogueInitiator : MonoBehaviour, IDialogueInitiator {
        [SerializeField, Required] private Dialogue m_Dialogue;
        [SerializeField, Required] private AdvancedLocomotionTeleport m_LocomotionTeleport;
        [SerializeField, Required] private TeleportPoint m_TeleportPoint;

        public event Action<Dialogue> DialogueInitiated;

        private void OnEnable() {
            m_LocomotionTeleport.TeleportedToPoint += OnTeleportedToPoint;
        }

        private void OnDisable() {
            m_LocomotionTeleport.TeleportedToPoint -= OnTeleportedToPoint;
        }

        private void OnTeleportedToPoint(TeleportPoint teleportPoint) {
            if (teleportPoint != m_TeleportPoint) {
                return;
            }

            DialogueInitiated?.Invoke(m_Dialogue);
        }
    }
}