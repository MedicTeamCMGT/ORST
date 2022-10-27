using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using ORST.Core.ModuleTasks;

namespace ORST.Core {
    public class PickupObjectTask : ModuleTask {
        [SerializeField] private GameObject m_ObjectToPick;
        private bool m_ObjectPickedUp;

        public override ModuleTaskState ExecuteModuleTask() {
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

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            m_ObjectPickedUp = pointerEvent.Type == PointerEventType.Select;
        }
    }
}
