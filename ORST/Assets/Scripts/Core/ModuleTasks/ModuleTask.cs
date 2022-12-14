using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks {
    public enum ModuleTaskState {
        Successful,
        Failure,
        Running
    }

    public class ModuleTask : MonoBehaviour {
        [SerializeField, InlineButton(nameof(FindModuleSubtasks), "Find Subtasks")] private List<ModuleTask> m_ModuleSubtasks;
        [SerializeField] private bool m_IsEligibleForRandom;

        private Queue<ModuleTask> m_ModuleSubtaskQueue;
        private ModuleTask m_CurrentModuleSubtask;
        private bool m_Started;
        private bool m_Completed;

        /// <summary>
        /// Gets a value indicating whether this task is eligible for randomization.
        /// </summary>
        public bool IsEligibleForRandom => m_IsEligibleForRandom;

        /// <summary>
        /// Gets a value indicating whether this task has started.
        /// </summary>
        public bool Started => m_Started;

        /// <summary>
        /// Gets a value indicating whether this task has completed.
        /// </summary>
        public bool Completed => m_Completed;

        private void Awake() {
            InitializeModuleTask();
        }

        private void FindModuleSubtasks() {
            m_ModuleSubtasks = new List<ModuleTask>(GetComponentsInChildren<ModuleTask>().Where(task => task != this));
        }

        private void InitializeModuleTask() {
            // On Subtasks this will be an empty list and will be skipped
            if (m_ModuleSubtasks.Count <= 0) {
                return;
            }

            m_ModuleSubtaskQueue = new Queue<ModuleTask>(m_ModuleSubtasks);
            Debug.Log("Task::Added subtasks: " + m_ModuleSubtaskQueue.Count);
            m_CurrentModuleSubtask = m_ModuleSubtaskQueue.Dequeue();
        }

        public void StartModuleTask() {
            if (m_CurrentModuleSubtask != null) {
                //We have subtasks, start subtask
                m_CurrentModuleSubtask.StartModuleTask();
                Debug.Log($"Task::Subtask '{m_CurrentModuleSubtask.gameObject.name}' started...");
            } else {
                Debug.Log($"Task::Task '{gameObject.name}' started...");
            }

            OnModuleTaskStarted();
            m_Completed = false;
            m_Started = true;
        }

        public ModuleTaskState UpdateModuleTask() {
            ModuleTaskState state = ExecuteModuleTask();
            if (state == ModuleTaskState.Successful) {
                m_Started = false;
                m_Completed = true;
                OnModuleTaskCompleted();
            }

            return state;
        }

        protected virtual void OnModuleTaskStarted() {
        }

        protected virtual void OnModuleTaskCompleted() {
        }

        protected virtual ModuleTaskState ExecuteModuleTask() {
            //Task implementation here, subtask will override it to implement functionality
            return AdvanceModuleSubtasks();
        }

        private ModuleTaskState AdvanceModuleSubtasks() {
            switch (m_CurrentModuleSubtask.ExecuteModuleTask()) {
                case ModuleTaskState.Successful:
                    //Subtask was successful
                    if (m_ModuleSubtaskQueue.Count > 0) {
                        Debug.Log($"Task::Subtask '{m_CurrentModuleSubtask.gameObject.name}' successful - Advancing...");
                        m_CurrentModuleSubtask = m_ModuleSubtaskQueue.Dequeue();
                        if (m_CurrentModuleSubtask != null) {
                            m_CurrentModuleSubtask.StartModuleTask();
                            //Not all subtasks all done - task returns running
                            return ModuleTaskState.Running;
                        }
                    } else {
                        //Subtasks all done - task returns successful
                        Debug.Log("Task::All subtasks done.");
                        return ModuleTaskState.Successful;
                    }
                    break;

                case ModuleTaskState.Failure:
                    //Subtask was failure
                    break;

                case ModuleTaskState.Running:
                    //Subtask is running
                    break;

                default:
                    throw new SwitchExpressionException(
                        "Task::Hit default case in 'AdvanceSubtask'. This should not happen.");
            }

            return ModuleTaskState.Failure;
        }

        //Note: Not perfect since this currently loses information by flattening the hierarchy
        public IEnumerable<ModuleTask> GetRemainingModuleTasks() {
            yield return this;

            if (m_ModuleSubtasks.Count <= 0) {
                yield break;
            }

            //First get the current task and its remaining subtasks
            foreach (ModuleTask task in m_CurrentModuleSubtask.GetRemainingModuleTasks()) {
                yield return task;
            }

            //Then fetch the ones still in the queue
            foreach (ModuleTask task in m_ModuleSubtaskQueue) {
                foreach (ModuleTask subTask in task.GetRemainingModuleTasks()) {
                    yield return subTask;
                }
            }
        }
    }
}