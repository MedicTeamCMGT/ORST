using System.Collections.Generic;
using ORST.Core.ModuleTasks;
using ORST.Foundation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ORST.Core.UI {
    public class TaskBoard : BaseMonoBehaviour {
        [SerializeField, Required] private List<TextMeshProUGUI> m_TaskLabels;

        private void Awake() {
            foreach (TextMeshProUGUI label in m_TaskLabels) {
                label.gameObject.SetActive(false);
                label.text = string.Empty;
            }
        }

        private void Start() {
            UpdateTaskBoard();
        }

        private void OnEnable() {
            ModuleTasksManager.Instance.TaskCompleted += OnTaskCompleted;
        }

        private void OnDisable() {
            ModuleTasksManager.Instance.TaskCompleted -= OnTaskCompleted;
        }

        private void OnTaskCompleted(ModuleTask task) => UpdateTaskBoard();

        private void UpdateTaskBoard() {
            List<ModuleTask> remainingTasks = ModuleTasksManager.Instance.GetRemainingTasks();
            int displayedCount = Mathf.Min(remainingTasks.Count, m_TaskLabels.Count);

            for (int i = 0; i < displayedCount; i++) {
                m_TaskLabels[i].text = remainingTasks[i].gameObject.name;
                m_TaskLabels[i].gameObject.SetActive(true);
            }

            for (int i = displayedCount; i < m_TaskLabels.Count; i++) {
                m_TaskLabels[i].gameObject.SetActive(false);
            }
        }
    }
}