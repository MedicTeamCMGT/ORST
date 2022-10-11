using UnityEngine;

namespace ORST.Core.Tasks
{
    public class Subtask_Test : Task {
        [SerializeField] private Transform TestTransform;
        private bool completed;
        
        private void Update() {
            if(!completed)ExampleTaskDefinition();
        }

        private void ExampleTaskDefinition() {
            if (TestTransform.position.y > 10.0f) {
                InvokeTaskEvent(TaskState.Successful);
                completed = true;
            }
        }
    }
}
