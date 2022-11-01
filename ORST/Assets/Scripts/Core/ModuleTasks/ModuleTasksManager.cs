using System.Collections.Generic;
using ORST.Foundation.Foundation.Extensions;
using ORST.Foundation.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    public class ModuleTasksManager : MonoSingleton<ModuleTasksManager> {
        [SerializeField] private List<ModuleTask> m_AllTasks;
        [SerializeField] private bool m_RandomizeEligibleModuleTasks;
        [ShowInInspector] private Queue<ModuleTask> m_TaskQueue;
        private ModuleTask m_CurrentModuleTask;

        private void Start() {
            InitiateModuleTaskManager();
        }

        private void Update() {
            //This stops execution when all tasks are done or there weren't tasks in the first place
            if (m_CurrentModuleTask == null) {
                return;
            }

            switch (m_CurrentModuleTask.UpdateModuleTask()) {
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
                IEnumerable<ModuleTask> allRemainingTasks = m_CurrentModuleTask.GetRemainingModuleTasks();

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

        public List<ModuleTask> GetRemainingTasks() {
            List<ModuleTask> remainingModuleTasks = new(m_TaskQueue.Count + 1) { m_CurrentModuleTask };
            remainingModuleTasks.AddRange(m_TaskQueue);
            return remainingModuleTasks;
        }

        private void InitiateModuleTaskManager() {
            List<ModuleTask> adjustedList = new();
            bool lastTaskRandomizable = false;
            if (m_RandomizeEligibleModuleTasks) {
                List<ModuleTask> randomModuleTasks = new();
                //Randomize task list before adding to queue
                foreach (ModuleTask currentTask in m_AllTasks) {
                    if (!currentTask.IsEligibleForRandom) {
                        if (lastTaskRandomizable) {
                            randomModuleTasks.Shuffle();
                            adjustedList.AddRange(randomModuleTasks);
                            randomModuleTasks.Clear();
                        }

                        lastTaskRandomizable = false;
                        adjustedList.Add(currentTask);
                    } else {
                        randomModuleTasks.Add(currentTask);
                        lastTaskRandomizable = true;
                    }
                }
            }

            m_TaskQueue = new Queue<ModuleTask>(m_RandomizeEligibleModuleTasks ? adjustedList : m_AllTasks);

            if (m_TaskQueue.Count > 0) {
                m_CurrentModuleTask = m_TaskQueue.Dequeue();
                m_CurrentModuleTask.StartModuleTask();
            }
        }
    }
}
