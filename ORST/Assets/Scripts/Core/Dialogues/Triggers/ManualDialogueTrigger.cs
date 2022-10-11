using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    public class ManualDialogueTrigger : MonoBehaviour, IDialogueTrigger {
        [SerializeField, Required] private Dialogue m_Dialogue;

        /// <summary>
        /// Manually initiate the dialogue.
        /// </summary>
        public void InitiateDialogue() {
            DialogueManager.StartDialogue(m_Dialogue);
        }
    }
}