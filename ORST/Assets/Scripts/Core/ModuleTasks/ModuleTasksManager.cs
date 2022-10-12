using System.Collections.Generic;
using UnityEngine;

namespace ORST.Core.ModuleTasks
{
    public class ModuleTasksManager : MonoBehaviour {

        [SerializeField] private List<ModuleTask> m_AllTasks;
        private Queue<ModuleTask> m_TaskQueue;
        private ModuleTask m_CurrentModuleTask;

        private void Start() {
            InitiateModuleTaskManager();
        }

        private void Update() {
            if (m_CurrentModuleTask == null) {
                return;
            }
            if (m_CurrentModuleTask.ExecuteModuleTask() == ModuleTaskState.Successful) {
                m_CurrentModuleTask = null;
                Debug.Log("Task::Task done.");
            }
        }

        private void InitiateModuleTaskManager() {
            m_TaskQueue = new Queue<ModuleTask>(m_AllTasks);
            if (m_TaskQueue.Count > 0) {
                m_CurrentModuleTask = m_TaskQueue.Dequeue();
                m_CurrentModuleTask.StartModuleTask();
            }
        }
    }
}
