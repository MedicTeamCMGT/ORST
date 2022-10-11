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
        Failure
    }

    public class Task : MonoBehaviour {

        public event Action<TaskState> TaskEvent;
        [SerializeField, InlineButton(nameof(FindSubtasks), "Find Subtasks")] private List<Task> m_Subtasks;
        [SerializeField] private bool IsEligibleForRandom;
        [ShowInInspector] private Queue<Task> m_SubtaskQueue;
        private Task m_CurrentSubtask;

        private void Start() {
            InitializeTask();
        }

        private void FindSubtasks() {
            m_Subtasks = new List<Task>(GetComponentsInChildren<Task>().Where(task => task != this));
        }

        private void InitializeTask() {
            //On Subtasks this will be an empty queue and will be skipped
            m_SubtaskQueue = new Queue<Task>(m_Subtasks);
            if (m_SubtaskQueue.Count > 0) {
                Debug.Log("Task::Added subtasks: " + m_SubtaskQueue.Count);
                m_CurrentSubtask = m_SubtaskQueue.Dequeue();
            }
        }

        private void RandomizeEligibleSubtasks() {
            //TODO: Implement randomization
            throw new NotImplementedException();
        }

        public virtual void StartTask(Task baseTask) {
            //For tasks this will be null
            //Tasks will use this implementation, while subtasks will override it to implement functionality.
            if (m_CurrentSubtask != null) {
                //We have subtasks
                m_CurrentSubtask.StartTask(this);
                m_CurrentSubtask.TaskEvent += AdvanceSubtasks;
            } else {
                Debug.Log("Task::Subtask started...");
            }
        }

        private void AdvanceSubtasks(TaskState taskState) {
            switch (taskState) {
                case TaskState.Successful:
                    if (m_SubtaskQueue.Count > 0) {
                        Debug.Log("Task::Task successful - Advancing...");
                        m_CurrentSubtask = m_SubtaskQueue.Dequeue();
                        if (m_CurrentSubtask != null) {
                            m_CurrentSubtask.StartTask(this);
                            m_CurrentSubtask.TaskEvent += AdvanceSubtasks;
                        }
                    } else {
                        //Subtasks all done
                        Debug.Log("Task::All subtasks done.");
                    }
                    break;

                case TaskState.Failure:
                    //E.g.: Restart subtasks or whole task
                    break;

                default:
                    throw new SwitchExpressionException("Task::Hit default case in task: " + gameObject.name);
            }
        }

        public void InvokeTaskEvent(TaskState taskState) {
            TaskEvent?.Invoke(taskState);
        }
    }
}
