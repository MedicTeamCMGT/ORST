using UnityEngine;
using Sirenix.OdinInspector;

namespace ORST.Core.ModuleTasks {
    public class WaitTask : ModuleTask {
        [SerializeField, SuffixLabel("seconds")] private float m_TimeToWait;
        private float m_TimePassed;

        protected override void OnModuleTaskStarted() {
            m_TimePassed = 0.0f;
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            //Note: ExecuteModuleTask is executed each frame.
            m_TimePassed += Time.deltaTime;
            return m_TimePassed >= m_TimeToWait ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }
    }
}