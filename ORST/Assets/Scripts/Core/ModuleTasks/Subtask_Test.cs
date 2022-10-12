using UnityEngine;

namespace ORST.Core.ModuleTasks
{
    public class Subtask_Test : ModuleTask {
        [SerializeField] private Transform TestTransform;

        public override ModuleTaskState ExecuteModuleTask() {
            if (TestTransform.transform.position.y > 10.0f) {
                return ModuleTaskState.Successful;
            }

            return ModuleTaskState.Running;
        }
    }
}
