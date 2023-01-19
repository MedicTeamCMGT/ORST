using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.ModuleTasks;
using ORST.Core.UI;
using ORST.Foundation.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace ORST.Core.Interactions {
    public class DoorZone : MonoBehaviour {
        public event Action ExitedDoor;
        [SerializeField, Required] private int m_SceneIndex;
        [SerializeField] private DoorInteractor m_DoorInteractor = DoorInteractor.Doorhandle;
        [SerializeField] private bool m_DisplayRemainingTasks = true;

        [Header("Door Handle")]
        [SerializeField] private HandGrabInteractable m_DoorHandle;

        [Header("Hand Sensor")]
        [SerializeField] private Transform m_HandSensor;
        [SerializeField, Range(0.2f, 10.0f)] private float m_SensorDetectionDistance;

        private OneGrabRotateTransformer m_DoorHandleRotateTransformer;
        private bool m_TransitionStarted;
        private GameObject m_Player;
        private Transform m_LeftHandTransform;
        private Transform m_RightHandTransform;

        private void Start() {
            if (m_DoorInteractor == DoorInteractor.Doorhandle) {
                Assert.IsNotNull(m_DoorHandle, "Door handle is null");
                m_DoorHandle.WhenPointerEventRaised += ProcessPointerEvent;

                // Skipping this assignment is valid since it is only used in the event handler method.
                m_DoorHandleRotateTransformer = m_DoorHandle.transform.GetComponent<OneGrabRotateTransformer>();

                m_HandSensor.OrNull()?.gameObject.SetActive(false);
            } else if (m_DoorInteractor == DoorInteractor.Sensor) {
                Assert.IsNotNull(m_HandSensor, "Hand sensor is null");
                // Cache hand references

                OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
                Assert.IsNotNull(cameraRig, "OVRCameraRig not found");

                m_LeftHandTransform = cameraRig.leftHandAnchor;
                m_RightHandTransform = cameraRig.rightHandAnchor;

                m_DoorHandle.OrNull()?.gameObject.SetActive(false);
            }
        }

        private void Update() {
            if (m_DoorInteractor != DoorInteractor.Sensor) {
                return;
            }

            CheckSensor();
        }

        private void OnDestroy() {
            if (m_DoorHandle != null) {
                m_DoorHandle.WhenPointerEventRaised -= ProcessPointerEvent;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player") || m_Player != null) {
                return;
            }

            m_Player = other.gameObject;
            if (!ModuleTasksManager.Instance.AllTasksCompleted) {
                if (!m_DisplayRemainingTasks) {
                    return;
                }

                List<ModuleTask> remainingTasks = ModuleTasksManager.Instance.GetRemainingTasks();
                PopupManager.Instance.DisplayTasks(remainingTasks);
                PopupManager.Instance.OpenPopup();
            }
        }

        private void OnTriggerExit(Collider other) {
            if (m_Player != null && m_Player == other.gameObject) {
                if (m_DisplayRemainingTasks) {
                    PopupManager.Instance.ClosePopup();
                }

                m_Player = null;
            }
        }

        private void CheckSensor() {
            if (ModuleTasksManager.Instance.AllTasksCompleted) {
                if ((m_HandSensor.position - m_LeftHandTransform.position).sqrMagnitude < m_SensorDetectionDistance * m_SensorDetectionDistance ||
                    (m_HandSensor.position - m_RightHandTransform.position).sqrMagnitude < m_SensorDetectionDistance * m_SensorDetectionDistance) {
                    InitiateSceneTransition();
                }
            }
        }

        private void InitiateSceneTransition() {
            if (m_TransitionStarted || !ModuleTasksManager.Instance.AllTasksCompleted) {
                return;
            }

            m_TransitionStarted = true;
            ExitedDoor?.Invoke();
            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene() {
            OVRScreenFade.instance.FadeOut();
            yield return new WaitUntil(() => Mathf.Abs(OVRScreenFade.instance.currentAlpha - 1.0f) < 0.01f);
            SceneManager.LoadScene(m_SceneIndex);
        }

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            if (pointerEvent.Type != PointerEventType.Move || m_TransitionStarted) {
                return;
            }

            if (!ModuleTasksManager.Instance.AllTasksCompleted) {
                return;
            }

            bool shouldConstrain = m_DoorHandleRotateTransformer.Constraints.MaxAngle.Constrain;
            if (shouldConstrain &&
                !Mathf.Approximately(m_DoorHandle.transform.rotation.eulerAngles.z, m_DoorHandleRotateTransformer.Constraints.MaxAngle.Value)) {
                return;
            }

            InitiateSceneTransition();
        }

        enum DoorInteractor {
            Doorhandle,
            Sensor
        }
    }
}
