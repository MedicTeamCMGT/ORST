using UnityEngine;
using ORST.Core.Movement;
using Sirenix.OdinInspector;

namespace ORST.Core.ModuleTasks {
    public class GoToTeleportPointTask : ModuleTask {
        [SerializeField, Required] private TeleportPointORST m_TeleportPoint;
        [LabelText("[?] Track Only When Running"), Tooltip("If this is true then the task will only update while the task is running.")]
        [SerializeField] private bool m_TrackOnlyWhenRunning = true;

        private bool m_Teleported;

        private void OnEnable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint += OnTeleportedToPoint;
        }

        private void OnDisable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint -= OnTeleportedToPoint;
        }

        private void OnTeleportedToPoint(TeleportPointORST teleportPoint) {
            if ((!m_TrackOnlyWhenRunning || Started) && teleportPoint == m_TeleportPoint) {
                m_Teleported = true;
            }
        }

        protected override void OnModuleTaskStarted() {
            if (m_TrackOnlyWhenRunning) {
                m_Teleported = false;
            }
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_Teleported ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }
    }
}