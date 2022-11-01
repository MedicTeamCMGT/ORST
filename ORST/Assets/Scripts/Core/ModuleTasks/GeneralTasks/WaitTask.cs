using UnityEngine;
using ORST.Core.ModuleTasks;
using Sirenix.OdinInspector;

namespace ORST.Core {
    public class WaitTask : ModuleTask {
        [SerializeField, SuffixLabel("seconds")] private float m_TimeToWait;
        private float m_TimePassed;

        protected override void OnModuleTaskStarted() {
            m_TimePassed = 0.0f;
        }

        public override ModuleTaskState ExecuteModuleTask() {
            //Note: ExecuteModuleTask is executed each frame.
            m_TimePassed += Time.deltaTime;
            return m_TimePassed >= m_TimeToWait ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }
    }
}