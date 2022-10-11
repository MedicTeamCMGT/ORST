using Sirenix.OdinInspector;

namespace ORST.Core.Dialogues.Demo {
    public class SampleDialogueTrigger : ManualDialogueTrigger {
        [ShowInInspector, ReadOnly] private bool m_DialogueStarted;

        private void OnEnable() {
            DialogueManager.DialogueEnded += OnDialogueEnded;
        }

        private void OnDisable() {
            DialogueManager.DialogueEnded -= OnDialogueEnded;
        }

        private void OnDialogueEnded(Dialogue dialogue) {
            if (dialogue == Dialogue) {
                m_DialogueStarted = false;
            }
        }

        /// <summary>
        /// Manually initiate the dialogue.
        /// </summary>
        public override void InitiateDialogue() {
            if (m_DialogueStarted) {
                return;
            }

            base.InitiateDialogue();
            m_DialogueStarted = true;
        }
    }
}