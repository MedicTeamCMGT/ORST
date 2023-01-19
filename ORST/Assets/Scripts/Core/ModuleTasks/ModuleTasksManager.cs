using System;
using System.Collections.Generic;
using System.Linq;
using ORST.Core.LearningModules;
using ORST.Foundation.Extensions;
using ORST.Foundation.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    [DefaultExecutionOrder(-10)]
    public class ModuleTasksManager : MonoSingleton<ModuleTasksManager> {
        [SerializeField] private List<ModuleTask> m_AllTasks;
        [SerializeField] private bool m_RandomizeEligibleModuleTasks;
        [ShowInInspector] private Queue<ModuleTask> m_TaskQueue;

        private ModuleTask m_CurrentModuleTask;
        private bool m_Completed;

        public event Action<ModuleTask> TaskStarted;
        public event Action<ModuleTask> TaskCompleted;

        public bool AllTasksCompleted => m_Completed;

        private void Start() {
            InitiateModuleTaskManager();
        }

        private void Update() {
            if (m_Completed) {
                return;
            }

            //This stops execution when all tasks are done or there weren't tasks in the first place
            if (m_CurrentModuleTask == null) {
                m_Completed = true;
                return;
            }

            switch (m_CurrentModuleTask.UpdateModuleTask()) {
                case ModuleTaskState.Successful:
                    TaskCompleted?.Invoke(m_CurrentModuleTask);

                    m_CurrentModuleTask = DequeueNextValidTask();
                    if (m_CurrentModuleTask != null) {
                        TaskStarted?.Invoke(m_CurrentModuleTask);

                        Debug.Log("TaskManager::Task successful - Advancing...");
                        m_CurrentModuleTask.StartModuleTask();
                    } else {
                        Debug.Log("TaskManager::All tasks done.");
                        m_Completed = true;
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
            List<ModuleTask> remainingModuleTasks = new(m_TaskQueue.Count + 1);
            if(m_CurrentModuleTask != null) {
                remainingModuleTasks.Add(m_CurrentModuleTask);
            }
            remainingModuleTasks.AddRange(m_TaskQueue.Where(ShouldStartTask));
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

            m_CurrentModuleTask = DequeueNextValidTask();
            if (m_CurrentModuleTask != null) {
                TaskStarted?.Invoke(m_CurrentModuleTask);
                m_CurrentModuleTask.StartModuleTask();
                return;
            }

            Debug.Log("TaskManager::No tasks to do.");
            m_Completed = true;
        }

        /// <summary>
        /// Returns <c>true</c> if the given <see cref="ModuleTask"/> should
        /// be started based on the selected <see cref="NameTagKind"/>.
        /// </summary>
        public static bool ShouldStartTask(ModuleTask moduleTask) {
            if (NameTag.Kind is NameTagKind.None) {
                return true;
            }

            if (moduleTask == null) {
                return false;
            }

            return moduleTask.NameTagRequirement is NameTagKind.None || moduleTask.NameTagRequirement == NameTag.Kind;
        }

        private ModuleTask DequeueNextValidTask() {
            if (m_TaskQueue.Count == 0) {
                return null;
            }

            ModuleTask nextTask = m_TaskQueue.Dequeue();
            while (!ShouldStartTask(nextTask)) {
                if (m_TaskQueue.Count == 0) {
                    return null;
                }

                nextTask = m_TaskQueue.Dequeue();
            }

            return nextTask;
        }
    }
}
