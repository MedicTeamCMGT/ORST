﻿using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.ModuleTasks;
using ORST.Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace ORST.Core.Interactions {
    public class DoorZone : MonoBehaviour {
        public event Action ExitedDoor;
        [SerializeField, Required] int m_SceneIndex;

        [SerializeField] private DoorInteractor m_DoorInteractor = DoorInteractor.Doorhandle;

        [Header("Door Handle")]
        [SerializeField, Required] private HandGrabInteractable m_DoorHandle;

        [Header("Hand Sensor")]
        [SerializeField, Required] private Transform m_HandSensor;
        [SerializeField, Range(0.2f, 10.0f)] private float m_SensorDetectionDistance;

        private OneGrabRotateTransformer m_DoorHandleRotateTransformer;
        private bool m_DoorsUnlocked;
        private bool m_TransitionStarted;
        private GameObject m_Player;
        private Transform m_DominantHand;
        private Transform m_NonDominantHand;
        private bool m_PlayerInZoneNoTasks;

        private void Start() {
            if (m_DoorInteractor == DoorInteractor.Doorhandle) {
                Assert.IsNotNull(m_DoorHandle, "Hand sensor is null");
                m_DoorHandle.WhenPointerEventRaised += ProcessPointerEvent;
                //Skipping this assignment is valid since it is only used in the event handler method.
                m_DoorHandleRotateTransformer = m_DoorHandle.transform.GetComponent<OneGrabRotateTransformer>();
            } else if (m_DoorInteractor == DoorInteractor.Sensor) {
                Assert.IsNotNull(m_HandSensor, "Hand sensor is null");
                //Cache hand references
                m_DominantHand = HandednessManager.DominantHand.transform;
                m_NonDominantHand = HandednessManager.NonDominantHand.transform;
            }
        }

        private void Update() {
            CheckSensor(); //Do bool check here -> less overhead
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
            List<ModuleTask> remainingTasks = ModuleTasksManager.Instance.GetRemainingTasks();
            m_DoorsUnlocked = remainingTasks.Count <= 0;
            if (!m_DoorsUnlocked) {
                PopupManager.Instance.DisplayTasks(remainingTasks);
                PopupManager.Instance.OpenPopup();
                return;
            }

            m_PlayerInZoneNoTasks = true;
        }

        private void OnTriggerExit(Collider other) {
            if (m_Player != null && m_Player == other.gameObject) {
                PopupManager.Instance.ClosePopup();
                m_Player = null;
                m_PlayerInZoneNoTasks = false;
            }
        }

        private void CheckSensor() {
            if (m_DoorInteractor != DoorInteractor.Sensor) {
                return;
            }

            if (m_PlayerInZoneNoTasks) {
                if ((m_HandSensor.position - m_DominantHand.position).magnitude < m_SensorDetectionDistance ||
                    (m_HandSensor.position - m_NonDominantHand.position).magnitude < m_SensorDetectionDistance) {
                    InitiateSceneTransition();
                }

            }
        }

        private void InitiateSceneTransition() {
            if (!m_DoorsUnlocked) {
                return;
            }

            m_PlayerInZoneNoTasks = false;
            m_TransitionStarted = true;
            ExitedDoor?.Invoke();
            StartCoroutine(ChangeScene());
        }

        private IEnumerator ChangeScene() {
            OVRScreenFade.instance.FadeOut();
            yield return new WaitUntil(() => OVRScreenFade.instance.currentAlpha >= 1.0f);
            SceneManager.LoadScene(m_SceneIndex);
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

            InitiateSceneTransition();
        }

        enum DoorInteractor {
            Doorhandle,
            Sensor
        }
    }
}
