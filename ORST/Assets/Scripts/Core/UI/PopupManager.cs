using System.Collections.Generic;
using ORST.Core.ModuleTasks;
using ORST.Foundation.Singleton;
using UnityEngine;

namespace ORST.Core.UI {
    public class PopupManager : MonoSingleton<PopupManager> {
        [SerializeField] private Canvas m_PopupCanvas;
        [SerializeField] private GameObject m_TaskPrefab;
        [SerializeField] private GameObject m_PopupInfoPrefab;
        [SerializeField] private Transform m_VerticalLayoutPanel;

        public void OpenPopup() {
            m_PopupCanvas.gameObject.SetActive(true);
        }

        public void ClosePopup() {
            m_PopupCanvas.gameObject.SetActive(false);
        }

        public bool IsPopupShown() {
            return m_PopupCanvas.gameObject.activeInHierarchy;
        }

        public void DisplayTasks(in List<ModuleTask> moduleTasksList) {
            DestroyVerticalPanelSubs();

            foreach (ModuleTask task in moduleTasksList) {
                Instantiate(m_TaskPrefab, m_VerticalLayoutPanel).GetComponent<TaskUI>().SetTaskTitle(task.name);
            }
        }

        public void DisplayInfo(string infoTitle, string infoMessage) {
            DestroyVerticalPanelSubs();
            PopupInfoUI popupInfo = Instantiate(m_PopupInfoPrefab, m_VerticalLayoutPanel).GetComponent<PopupInfoUI>();
            popupInfo.SetInfoTitle(infoTitle);
            popupInfo.SetInfoMessage(infoMessage);
        }

        private void DestroyVerticalPanelSubs() {
            for (int i = m_VerticalLayoutPanel.childCount - 1; i >= 0; i--) {
                Destroy(m_VerticalLayoutPanel.GetChild(i).gameObject);
            }
        }
    }
}
