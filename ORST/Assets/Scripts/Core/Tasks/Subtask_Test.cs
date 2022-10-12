using UnityEngine;

namespace ORST.Core.Tasks
{
    public class Subtask_Test : Task {
        [SerializeField] private Transform TestTransform;

        public override TaskState ExecuteTask() {
            if (TestTransform.transform.position.y > 10.0f) {
                return TaskState.Successful;
            }

            return TaskState.Running;
        }
    }
}
