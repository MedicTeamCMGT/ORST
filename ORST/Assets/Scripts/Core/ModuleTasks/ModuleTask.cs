using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks
{
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

        private void Start() {
            InitializeModuleTask();
        }

        private void FindModuleSubtasks() {
            m_ModuleSubtasks = new List<ModuleTask>(GetComponentsInChildren<ModuleTask>().Where(task => task != this));
        }

        private void InitializeModuleTask() {
            //On Subtasks this will be an empty list and will be skipped
            if (m_ModuleSubtasks.Count <= 0) {
                return;
            }

            m_ModuleSubtaskQueue = new Queue<ModuleTask>(m_ModuleSubtasks);
            Debug.Log("Task::Added subtasks: " + m_ModuleSubtaskQueue.Count);
            m_CurrentModuleSubtask = m_ModuleSubtaskQueue.Dequeue();
        }

        private void RandomizeEligibleSubtasks() {
            //TODO: Implement randomization
            throw new NotImplementedException();
        }

        public void StartModuleTask() {
            if (m_CurrentModuleSubtask != null) {
                //We have subtasks, start subtask
                m_CurrentModuleSubtask.StartModuleTask();
            } else {
                Debug.Log("Task::Subtask started...");
            }
        }

        public virtual ModuleTaskState ExecuteModuleTask() {
            //Task implementation here, subtask will override it to implement functionality
            return AdvanceModuleSubtasks();
        }

        private ModuleTaskState AdvanceModuleSubtasks() {
            switch (m_CurrentModuleSubtask.ExecuteModuleTask()) {
                case ModuleTaskState.Successful:
                    if (m_ModuleSubtaskQueue.Count > 0) {
                        Debug.Log("Task::Task successful - Advancing...");
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
    }
}
