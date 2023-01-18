using Oculus.Interaction.Input;
using ORST.Core.Interactions;
using ORST.Core.ModuleTasks;
using UnityEngine;

namespace ORST.Core {
    public class EnterHandednessTask : ModuleTask {
        [SerializeField] private GameObject m_EnterHandednessRoot;
        private bool m_ConfirmedHandedness;

        protected override void OnModuleTaskStarted() {
            base.OnModuleTaskStarted();
            m_EnterHandednessRoot.gameObject.SetActive(true);
            HandednessManager.HandednessChanged += OnHandednessChanged;
        }

        protected override void OnModuleTaskCompleted() {
            base.OnModuleTaskCompleted();
            m_EnterHandednessRoot.gameObject.SetActive(false);
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_ConfirmedHandedness ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }

        private void OnHandednessChanged(Handedness handedness) {
            m_ConfirmedHandedness = true;
        }
    }
}