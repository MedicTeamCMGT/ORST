using ORST.Core.Movement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    public class TeleportPointDialogueTrigger : MonoBehaviour, IDialogueTrigger {
        [SerializeField, Required] private Dialogue m_Dialogue;
        [SerializeField, Required] private AdvancedLocomotionTeleport m_LocomotionTeleport;
        [SerializeField, Required] private TeleportPointORST m_TeleportPoint;


        private void OnEnable() {
            m_LocomotionTeleport.TeleportedToPoint += OnTeleportedToPoint;
        }

        private void OnDisable() {
            m_LocomotionTeleport.TeleportedToPoint -= OnTeleportedToPoint;
        }

        private void OnTeleportedToPoint(TeleportPointORST teleportPoint) {
            if (teleportPoint != m_TeleportPoint) {
                if (ReferenceEquals(DialogueManager.ActiveDialogue, m_Dialogue)) {
                    // If you teleport away while the dialogue is active then it will be considered
                    // cancelled instead of completed.
                    DialogueManager.EndDialogue(false);
                }

                return;
            }

            DialogueManager.StartDialogue(m_Dialogue);
        }
    }
}