using UnityEngine;

namespace ORST.Core.Dialogues.Demo {
    public class SampleDialogueTrigger : ManualDialogueTrigger {
        [Tooltip("If this is enabled then the trigger will only work for the first time.")]
        [SerializeField] private bool m_AllowOnlyOnce;

        private bool m_DialogueStarted;
        private bool m_CanTriggerDialogue = true;

        private void OnEnable() {
            DialogueManager.DialogueEnded += OnDialogueEnded;
        }

        private void OnDisable() {
            DialogueManager.DialogueEnded -= OnDialogueEnded;
        }

        private void OnDialogueEnded(Dialogue dialogue, bool completed) {
            if (dialogue == Dialogue) {
                m_DialogueStarted = false;
            }
        }

        /// <summary>
        /// Manually initiate the dialogue.
        /// </summary>
        public override void InitiateDialogue() {
            if (m_DialogueStarted || !m_CanTriggerDialogue) {
                return;
            }

            if (m_AllowOnlyOnce) {
                m_CanTriggerDialogue = false;
            }

            base.InitiateDialogue();
            m_DialogueStarted = true;
        }
    }
}