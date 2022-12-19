using ORST.Core.LearningModules;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    public class PickUpNameTagTask : ModuleTask {
        [SerializeField, Required] private NameTagModuleManager m_NameTagModuleManager;
        private bool m_NameTagConfirmed;

        protected override void OnModuleTaskStarted() {
            base.OnModuleTaskStarted();

            m_NameTagModuleManager.NameTagConfirmed += OnNameTagConfirmed;
            m_NameTagModuleManager.HandleTaskStarted();
        }

        protected override void OnModuleTaskCompleted() {
            base.OnModuleTaskCompleted();

            m_NameTagModuleManager.NameTagConfirmed -= OnNameTagConfirmed;
        }

        private void OnDestroy() {
            if (m_NameTagModuleManager != null) {
                m_NameTagModuleManager.NameTagConfirmed -= OnNameTagConfirmed;
            }
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_NameTagConfirmed ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }

        private void OnNameTagConfirmed() {
            m_NameTagConfirmed = true;
        }
    }
}