using System;
using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.Dialogues;
using ORST.Core.Interactions;
using ORST.Core.Movement;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace ORST.Core.ModuleTasks {
    /// <summary>
    /// A custom (hardcoded) implementation of the ORST tutorial.
    /// </summary>
    ///
    public class TutorialTasks : MonoBehaviour {
        public Dialogue Dialogue => m_Dialogue;
        [Header("Dialogue")]
        [SerializeField] private Dialogue m_Dialogue;
        [SerializeField, Required] private DialogueNPC m_NPC;
        [SerializeField, Required] private DialogueView m_DialogueViewPrefab;

        [ShowInInspector] private DialogueState m_State;
        //[ShowInInspector] private bool m_FinishedInteractingWithUI;
        [ShowInInspector] private DialogueView m_DialogueView;

        [Space(20)]
        [Header("Teleport Gesture")]
        [SerializeField] private Transform m_GestureCanvasPosition;

        [Space(20)]
        [Header("Teleportation")]
        [SerializeField] private TeleportPointORST m_TeleportPoint;
        [SerializeField] private Transform m_TeleportCanvasPosition;
        [SerializeField] private TeleportInputHandlerHands m_TeleportInputHandlerHands;

        [Space(20)]
        [Header("Object Pick-up")]
        [SerializeField] private TeleportPointORST m_PickupPoint;
        [SerializeField] private Transform m_PickupCanvasPosition;
        [SerializeField, Required] private GameObject m_ObjectToPick;

        [Space(20)]
        [Header("Door")]
        [SerializeField] private TeleportPointORST m_DoorPoint;
        [SerializeField] private Transform m_DoorCanvasPosition;
        [SerializeField] private DoorZone m_DoorZone;

        private bool m_ObjectPickedUp;
        private bool m_DoorOpened;
        private bool m_Started;

        private bool m_TutorialDecisionMade;
        private TeleportPointORST m_EnabledTeleportPoint;
        private bool m_TeleportedToEnabledPoint;

        private void Start() {
            DialogueManager.StartDialogue(m_Dialogue);
            HandGrabInteractable interactable = m_ObjectToPick.GetComponentInChildren<HandGrabInteractable>();
            if (interactable != null) {
                interactable.WhenPointerEventRaised += ProcessPointerEvent;
            }

            m_DoorZone.ExitedDoor += CheckDoorOpened;
            m_DialogueView.transform.position = m_GestureCanvasPosition.position;
            m_DialogueView.transform.rotation = m_GestureCanvasPosition.rotation;
        }

        public void HandleDialogueStarted() {
            Assert.AreEqual(DialogueManager.ActiveDialogue, Dialogue);

            m_State = new DialogueState(Dialogue);
            Assert.IsTrue(m_State.Advance(), "m_State.Advance() returned false; the dialogue is empty.");

            m_DialogueView = Instantiate(m_DialogueViewPrefab);
            m_DialogueView.gameObject.SetActive(true);
            m_DialogueView.Initialize(m_NPC, AccessTutorial);
            m_DialogueView.LoadState(m_State.CurrentNode);
        }


        public void HandleDialogueEnded(bool completed) {
            Debug.Log($"Dialogue ended. Completed: {completed}");
            gameObject.SetActive(false);
        }

        private void AccessTutorial(int optionIndex) {
            m_DialogueView.LoadState(m_State.CurrentNode);

            if (optionIndex == 1) {
                //Close tutorial
                TerminateDialogue();
            } else {
                StartCoroutine(Tutorial());
            }

            if (!m_State.Advance()) {
             TerminateDialogue();
            }
        }

        private void AdvanceTutorial() {
            
        }

        private void TerminateDialogue() {
            Destroy(m_DialogueView.gameObject);
            m_DialogueView = null;
            DialogueManager.EndDialogue(false);
            m_State = default;
        }

        private IEnumerator Tutorial() {
            //Setup
            TeleportPointManager.DisableAllPoints();
            DisableCurrentTeleportPoint.Instance.IsDisabled = true;
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint += CheckActiveTeleportPoint;
            yield return null;

            //Task 1
            yield return ValidateTask(m_GestureCanvasPosition, null,
                               () => m_TeleportInputHandlerHands.GetIntention() == LocomotionTeleport.TeleportIntentions.Aim, 3.0f);
            //Task 2
            yield return ValidateTask(m_TeleportCanvasPosition, m_TeleportPoint, () => m_TeleportedToEnabledPoint, awaitTeleport:false);

            //Task 3
            m_Started = true;
            yield return ValidateTask(m_PickupCanvasPosition, m_PickupPoint, () => m_ObjectPickedUp);

            //Task 4
            yield return ValidateTask(m_DoorCanvasPosition, m_DoorPoint, () => m_DoorOpened);

            //Cleanup
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint -= CheckActiveTeleportPoint;
            DisableCurrentTeleportPoint.Instance.IsDisabled = false;
            TeleportPointManager.EnableAllPoints();
            ResetTutorial();
        }

        private IEnumerator ValidateTask(Transform canvasTransform, TeleportPointORST teleportPoint, Func<bool> waitUntil,
                                  float waitDelay = 0.0f, bool continueDialogue = true, bool awaitTeleport = true) {
            if (teleportPoint != null) {
                TeleportPointManager.EnablePoint(teleportPoint);
                m_EnabledTeleportPoint = teleportPoint;
                if (awaitTeleport) {
                    yield return new WaitUntil(() => m_TeleportedToEnabledPoint);
                }
            }

            Transform dialogueTransform = m_DialogueView.transform;
            dialogueTransform.position = canvasTransform.position;
            dialogueTransform.rotation = canvasTransform.rotation;
            m_DialogueView.gameObject.SetActive(true);
            if (continueDialogue) {
                AccessTutorial(0);
            }

            yield return new WaitForSeconds(waitDelay);
            yield return new WaitUntil(waitUntil);

            if (teleportPoint != null) {
                TeleportPointManager.DisablePoint(teleportPoint);
            }

            m_EnabledTeleportPoint = null;
            m_TeleportedToEnabledPoint = false;
            m_DialogueView.gameObject.SetActive(false);
        }

        private void CheckActiveTeleportPoint(TeleportPointORST teleportPoint) {
            if (m_EnabledTeleportPoint != null && m_EnabledTeleportPoint == teleportPoint) {
                m_TeleportedToEnabledPoint = true;
            }
        }

        private void CheckDoorOpened() {
            if (m_Started) {
                m_DoorOpened = true;
            }
        }

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            if (m_Started && pointerEvent.Type == PointerEventType.Select) {
                m_ObjectPickedUp = true;
            }
        }

        private void ResetTutorial() {
            m_DoorOpened = false;
        }
    }
}
