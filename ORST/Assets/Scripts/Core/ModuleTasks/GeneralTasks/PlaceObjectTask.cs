using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    public class PlaceObjectTask : ModuleTask {
        [SerializeField, Required] private GameObject m_ObjectToPlace;
        [LabelText("[?] Track Only When Running"), Tooltip("If this is enabled then the task will only update while the task is running.")]
        [SerializeField] private bool m_TrackOnlyWhenRunning = true;
        private bool m_PlacedObject;
        private BoxCollider m_BoxCollider;

        protected override void OnModuleTaskStarted() {
            if (m_TrackOnlyWhenRunning) {
                m_PlacedObject = false;
            }
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_PlacedObject ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }

        private void Reset() {
            m_BoxCollider ??= gameObject.AddComponent<BoxCollider>();
            m_BoxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other) {
            if ((!m_TrackOnlyWhenRunning || Started) && other.gameObject == m_ObjectToPlace) {
                m_PlacedObject = true;
            }
        }
    }
}
