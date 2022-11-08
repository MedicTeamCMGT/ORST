using ORST.Core.Dialogues;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    public class InitiateDialogueTask : ModuleTask {
        [SerializeField, Required] private Dialogue m_Dialogue;
        [LabelText("[?] Track Only When Running"), Tooltip("If this is true then the task will only update while the task is running.")]
        [SerializeField] private bool m_TrackOnlyWhenRunning = true;

        private bool m_Completed;

        private void OnEnable() {
            DialogueManager.DialogueStarted += OnDialogueStarted;
        }

        private void OnDisable() {
            DialogueManager.DialogueStarted -= OnDialogueStarted;
        }

        protected override void OnModuleTaskStarted() {
            if (m_TrackOnlyWhenRunning) {
                m_Completed = false;
            }
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_Completed ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }

        private void OnDialogueStarted(Dialogue dialogue) {
            if (m_TrackOnlyWhenRunning && !Started) {
                return;
            }

            if (dialogue == m_Dialogue) {
                m_Completed = true;
            }
        }
    }
}