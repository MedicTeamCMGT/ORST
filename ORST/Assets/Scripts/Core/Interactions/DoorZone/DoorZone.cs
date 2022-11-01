using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.ModuleTasks;
using ORST.Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ORST.Core.Interactions {
    public class DoorZone : MonoBehaviour {
        [SerializeField, Required] private HandGrabInteractable m_DoorHandle;
        private OneGrabRotateTransformer m_DoorHandleRotateTransformer;
        private bool m_DoorsUnlocked;
        private bool m_TransitionStarted;
        private GameObject m_Player;

        private void Start() {
            if (m_DoorHandle != null) {
                m_DoorHandle.WhenPointerEventRaised += ProcessPointerEvent;
                m_DoorHandleRotateTransformer = m_DoorHandle.transform.GetComponent<OneGrabRotateTransformer>();
            }
        }

        private void OnDestroy() {
            if (m_DoorHandle != null) {
                m_DoorHandle.WhenPointerEventRaised -= ProcessPointerEvent;
            }
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
            OVRScreenFade.instance.FadeOut();
            yield return new WaitUntil(() => OVRScreenFade.instance.currentAlpha >= 1.0f);
            SceneManager.LoadScene(10);
        }

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            if (pointerEvent.Type != PointerEventType.Move || m_TransitionStarted) {
                return;
            }

            if (m_DoorHandleRotateTransformer.Constraints.MaxAngle.Constrain &&
                !Mathf.Approximately(m_DoorHandle.transform.rotation.eulerAngles.z,
                                     m_DoorHandleRotateTransformer.Constraints.MaxAngle.Value)) {
                return;
            }

            StartCoroutine(ChangeScene());
            m_TransitionStarted = true;
        }
    }
}
