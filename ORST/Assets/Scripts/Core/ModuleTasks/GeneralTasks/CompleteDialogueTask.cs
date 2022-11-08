using ORST.Core.Dialogues;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    public class CompleteDialogueTask : ModuleTask {
        [SerializeField, Required] private Dialogue m_Dialogue;
        [LabelText("[?] Track Only When Running"), Tooltip("If this is true then the task will only update while the task is running.")]
        [SerializeField] private bool m_TrackOnlyWhenRunning = true;

        private ModuleTaskState m_CurrentState = ModuleTaskState.Running;

        private void OnEnable() {
            DialogueManager.DialogueEnded += OnDialogueEnded;
        }

        private void OnDisable() {
            DialogueManager.DialogueEnded -= OnDialogueEnded;
        }

        protected override void OnModuleTaskStarted() {
            if (m_TrackOnlyWhenRunning) {
                m_CurrentState = ModuleTaskState.Running;
            }
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_CurrentState;
        }

        private void OnDialogueEnded(Dialogue dialogue, bool completed) {
            if (m_TrackOnlyWhenRunning && !Started) {
                return;
            }

            if (dialogue != m_Dialogue) {
                return;
            }

            m_CurrentState = completed ? ModuleTaskState.Successful : ModuleTaskState.Failure;
        }
    }
}