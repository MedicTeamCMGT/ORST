using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using ORST.Core.ModuleTasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ORST.Core.Interactions {
    public class DoorZone : MonoBehaviour {
        private bool m_DoorsUnlocked;
        private Grabbable m_DoorHandle;
        private GameObject m_Player;

        private void Start() {
            m_DoorHandle = GetComponent<Grabbable>();
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) {
                return;
            }

            m_Player = other.gameObject;
            List<ModuleTask> remainingTasks = ModuleTasksManager.Instance.GetRemainingTasks();
            m_DoorsUnlocked = remainingTasks.Count <= 0;
            if (!m_DoorsUnlocked) {
                PopupManager.Instance.DisplayTasks(remainingTasks);
                PopupManager.Instance.OpenPopup();
            }

            DoorHandleMoved();
        }

        private void OnTriggerExit(Collider other) {
            if (m_Player == other.gameObject) {
                PopupManager.Instance.ClosePopup();
            }
        }

        public void DoorHandleMoved() {
            Debug.LogWarning("Teleport to next room");
            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene() {
            yield return new WaitForSeconds(15);
            OVRScreenFade.instance.FadeOut();
            yield return new WaitUntil(() => OVRScreenFade.instance.currentAlpha >= 1.0f);
            SceneManager.LoadScene(10);
        }
    }
}
