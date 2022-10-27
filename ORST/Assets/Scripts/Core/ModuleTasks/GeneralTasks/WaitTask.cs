using UnityEngine;
using ORST.Core.ModuleTasks;

namespace ORST.Core {
    public class WaitTask : ModuleTask {
        [SerializeField] private float m_TimeToWait;
        private float m_TimePassed;

        public override ModuleTaskState ExecuteModuleTask() {
            //Note: ExecuteModuleTask is executed each frame.
            m_TimePassed += Time.deltaTime;
            return m_TimePassed >= m_TimeToWait ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }
    }
}
