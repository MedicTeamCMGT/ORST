using TMPro;
using UnityEngine;

namespace ORST.Core.UI {
    public class TaskUI : MonoBehaviour {
        [SerializeField] private TMP_Text m_TaskTitle;

        public void SetTaskTitle(string taskTitle) {
            m_TaskTitle.text = taskTitle;
        }
    }
}
