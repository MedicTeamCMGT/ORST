using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Tasks
{
    public enum TaskState {
        Successful,
        Failure,
        Running
    }

    public class Task : MonoBehaviour {
        [SerializeField, InlineButton(nameof(FindSubtasks), "Find Subtasks")] private List<Task> m_Subtasks;
        [SerializeField] private bool m_IsEligibleForRandom;

        private Queue<Task> m_SubtaskQueue;
        private Task m_CurrentSubtask;

        private void Start() {
            InitializeTask();
        }

        private void FindSubtasks() {
            m_Subtasks = new List<Task>(GetComponentsInChildren<Task>().Where(task => task != this));
        }

        private void InitializeTask() {
            //On Subtasks this will be an empty list and will be skipped
            if (m_Subtasks.Count <= 0) {
                return;
            }

            m_SubtaskQueue = new Queue<Task>(m_Subtasks);
            Debug.Log("Task::Added subtasks: " + m_SubtaskQueue.Count);
            m_CurrentSubtask = m_SubtaskQueue.Dequeue();
        }

        private void RandomizeEligibleSubtasks() {
            //TODO: Implement randomization
            throw new NotImplementedException();
        }

        public void StartTask() {
            if (m_CurrentSubtask != null) {
                //We have subtasks, start subtask
                m_CurrentSubtask.StartTask();
            } else {
                Debug.Log("Task::Subtask started...");
            }
        }

        public virtual TaskState ExecuteTask() {
            //Task implementation here, subtask will override it to implement functionality
            return AdvanceSubtasks();
        }

        private TaskState AdvanceSubtasks() {
            switch (m_CurrentSubtask.ExecuteTask()) {
                case TaskState.Successful:
                    if (m_SubtaskQueue.Count > 0) {
                        Debug.Log("Task::Task successful - Advancing...");
                        m_CurrentSubtask = m_SubtaskQueue.Dequeue();
                        if (m_CurrentSubtask != null) {
                            m_CurrentSubtask.StartTask();
                            //Not all subtasks all done - task returns running
                            return TaskState.Running;
                        }
                    } else {
                        //Subtasks all done - task returns successful
                        Debug.Log("Task::All subtasks done.");
                        return TaskState.Successful;
                    }
                    break;

                case TaskState.Failure:
                    //Subtask was failure
                    break;

                case TaskState.Running:
                    //Subtask is running
                    break;

                default:
                    throw new SwitchExpressionException(
                        "Task::Hit default case in 'AdvanceSubtask'. This should not happen.");
            }

            return TaskState.Failure;
        }
    }
}
