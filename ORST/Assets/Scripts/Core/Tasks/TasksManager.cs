using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ORST.Core.Tasks
{
    public class TasksManager : MonoBehaviour {

        private Queue<Task> m_TaskQueue;
        [SerializeField] private List<Task> m_AllTasks;

        private void Start() {
            //Fetch all tasks available in the scene.
        }
    }
}
