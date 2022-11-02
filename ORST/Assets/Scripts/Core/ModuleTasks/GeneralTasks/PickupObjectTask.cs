using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ORST.Core.ModuleTasks {
    public class PickupObjectTask : ModuleTask {
        [SerializeField, Required] private GameObject m_ObjectToPick;
        [LabelText("[?] Track Only When Running"), Tooltip("If this is enabled then the task will only update while the task is running.")]
        [SerializeField] private bool m_TrackOnlyWhenRunning = true;

        private bool m_ObjectPickedUp;

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_ObjectPickedUp ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }

        private void Start() {
            HandGrabInteractable interactable = m_ObjectToPick.GetComponentInChildren<HandGrabInteractable>();
            if (interactable != null) {
                interactable.WhenPointerEventRaised += ProcessPointerEvent;
                return;
            }

            Debug.Log("Task::Object in 'PickupObjectTask' does not contain 'HandGrabInteractable' component.");
        }

        protected override void OnModuleTaskStarted() {
            if (m_TrackOnlyWhenRunning) {
                m_ObjectPickedUp = false;
            }
        }

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            if ((!m_TrackOnlyWhenRunning || Started) && pointerEvent.Type == PointerEventType.Select) {
                m_ObjectPickedUp = true;
            }
        }
    }
}