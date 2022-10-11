using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core
{
    public class Task : MonoBehaviour {
        [SerializeField, InlineButton(nameof(FindSubtasks), "Find")] private List<Task> m_Subtasks;

        private void FindSubtasks() {
            m_Subtasks = new List<Task>(GetComponentsInChildren<Task>().Where(task => task != this));
        }
    }
}
