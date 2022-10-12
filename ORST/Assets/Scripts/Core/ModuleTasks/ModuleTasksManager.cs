using System.Collections.Generic;
using ORST.Foundation.Singleton;
using UnityEngine;

namespace ORST.Core.ModuleTasks
{
    public class ModuleTasksManager : MonoSingleton<ModuleTasksManager> {

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

            switch (m_CurrentModuleTask.ExecuteModuleTask()) {
                case ModuleTaskState.Successful:
                    //Task was successful
                    if (m_TaskQueue.Count > 0) {
                        Debug.Log("TaskManager::Task successful - Advancing...");
                        m_CurrentModuleTask = m_TaskQueue.Dequeue();
                        m_CurrentModuleTask.StartModuleTask();
                    } else {
                        m_CurrentModuleTask = null;
                        Debug.Log("TaskManager::All tasks done.");
                    }
                    break;
                case ModuleTaskState.Failure:
                    break;
                case ModuleTaskState.Running:
                    break;
            }

            if (Input.GetKeyDown(KeyCode.O)) {
                List<ModuleTask> allRemainingTasks = m_CurrentModuleTask.GetRemainingModuleTasks();

                foreach (ModuleTask task in allRemainingTasks) {
                    Debug.Log("Task::Task: " + task.gameObject.name);
                }
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                foreach (ModuleTask task in GetRemainingTasks()) {
                    Debug.Log("TaskManager::Task: " + task.gameObject.name);
                }
            }
        }

        private List<ModuleTask> GetRemainingTasks() {
            List<ModuleTask> remainingModuleTasks = new() { m_CurrentModuleTask };
            remainingModuleTasks.AddRange(m_TaskQueue);
            return remainingModuleTasks;
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
