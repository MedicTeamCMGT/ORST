using System.Collections.Generic;
using UnityEngine;

namespace ORST.Core.Tasks
{
    public class TasksManager : MonoBehaviour {

        [SerializeField] private List<Task> m_AllTasks;
        private Queue<Task> m_TaskQueue;
        private Task m_CurrentTask;

        private void Start() {
            InitiateTaskManager();
        }

        private void InitiateTaskManager() {
            m_TaskQueue = new Queue<Task>(m_AllTasks);
            if (m_TaskQueue.Count > 0) {
                m_CurrentTask = m_TaskQueue.Dequeue();
                m_CurrentTask.StartTask(null);
            }
        }
    }
}
