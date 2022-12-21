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
using ORST.Core.UI;
using ORST.Core.Dialogues.Demo;

namespace ORST.Core.ModuleTasks {
    /// <summary>
    /// A custom (hardcoded) implementation of the ORST tutorial.
    /// </summary>
    public class TutorialDialogueHandler : MonoBehaviour {
        public Action<bool> TutorialCompleted;

        //Dialogue Manager
        public TutorialDialogue Dialogue => m_Dialogue;
        [Header("Dialogue")]
        [SerializeField] private TutorialDialogue m_Dialogue;
        [SerializeField, Required] private TutorialDialogueView m_DialogueViewPrefab;
        [ShowInInspector] private TutorialDialogueState m_State;
        [ShowInInspector] private TutorialDialogueView m_DialogueView;

        //Tutorial Tasks
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
        [SerializeField, Required] private ItemBucket m_ItemBucket;

        [Space(20)]
        [Header("Menu Interaction")]
        [SerializeField] private TeleportPointORST m_MenuPoint;
        [SerializeField] private Transform m_MenuCanvasPosition;
        [SerializeField] private DialogueHandler m_ExampleDialogue;

        [Space(20)]
        [Header("Door")]
        [SerializeField] private TeleportPointORST m_DoorPoint;
        [SerializeField] private Transform m_DoorCanvasPosition;
        [SerializeField] private DoorZone m_DoorZone;

        private bool m_ObjectPickedUp;
        private bool m_ItemsInBucket;
        private bool m_DoorOpened;
        private bool m_Started;
        private bool m_ButtonClicked;
        private bool m_DialogueEnded;

        private TeleportPointORST m_EnabledTeleportPoint;
        private bool m_TeleportedToEnabledPoint;

        private SampleDialogueTrigger m_SampleDialogueTrigger;
        public void StartTutorial() {
            //Note: The tutorial implementation should start itself, currently only one exists.
            //Can be adjusted to facilitate multiple handlers if so desired.
            HandGrabInteractable interactable = m_ObjectToPick.GetComponentInChildren<HandGrabInteractable>();
            if (interactable != null) {
                interactable.WhenPointerEventRaised += ProcessPointerEvent;
            }

            m_SampleDialogueTrigger = GetComponent<SampleDialogueTrigger>();

            HandleDialogueStarted();
            m_DoorZone.ExitedDoor += CheckDoorOpened;
            //Put this in a separate method.
            m_DialogueView.transform.position = m_GestureCanvasPosition.position;
            m_DialogueView.transform.rotation = m_GestureCanvasPosition.rotation;

            if (!m_Started) {
                StartCoroutine(Tutorial());
            }
        }

        private void HandleDialogueStarted() {
            m_State = new TutorialDialogueState(Dialogue);
            Assert.IsTrue(m_State.Advance(), "m_State.Advance() returned false; the dialogue is empty.");
            InitializeView();
        }

        public void HandleDialogueEnded() {
            //Cleanup
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint -= CheckActiveTeleportPoint;
            DisableCurrentTeleportPoint.Instance.IsDisabled = false;
            TeleportPointManager.EnableAllPoints();
            ResetTutorial();
            gameObject.SetActive(false);
        }

        private void AdvanceTutorial() {
            if (!m_State.Advance()) {
                TerminateDialogue();
                return;
            }
            m_DialogueView.LoadState(m_State.CurrentNode);
        }

        private void TerminateDialogue() {
            HandleDialogueEnded();
            Destroy(m_DialogueView.gameObject);
            m_DialogueView = null; //Calls 'HandleDialogueEnded' on completion.
            m_State = default;
        }

        private void ViewButtonClicked() {
            m_ButtonClicked = true;
        }

        private void InitializeTutorial() {
            m_DialogueView.DisableButton();
            TeleportPointManager.DisableAllPoints();
            DisableCurrentTeleportPoint.Instance.IsDisabled = true;
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint += CheckActiveTeleportPoint;
        }

        private void InitializeView() {
            m_DialogueView = Instantiate(m_DialogueViewPrefab);
            m_DialogueView.LoadState(m_State.CurrentNode);
            m_DialogueView.gameObject.SetActive(true);
            m_DialogueView.Initialize(ViewButtonClicked);
        }

        private IEnumerator Tutorial() {
            InitializeTutorial();

            //Task - Teleportation Gesture
            yield return ValidateTask(m_GestureCanvasPosition, null,
                               () => m_TeleportInputHandlerHands.GetIntention() == LocomotionTeleport.TeleportIntentions.Aim, 3.0f);

            //Task - Teleportation
            yield return ValidateTask(m_TeleportCanvasPosition, m_TeleportPoint, () => m_TeleportedToEnabledPoint, awaitTeleport: false);

            //Task - Pickup
            m_Started = true;
            yield return ValidateTask(m_PickupCanvasPosition, null, () => m_ObjectPickedUp, awaitTeleport: false);

            //Task - Throw away
            m_ItemBucket.ItemBucketFilled += () => m_ItemsInBucket = true;
            yield return ValidateTask(m_PickupCanvasPosition, null, () => m_ItemsInBucket);

            //Task - Throw away - Response
            yield return ValidateTask(m_PickupCanvasPosition, m_MenuPoint, () => m_TeleportedToEnabledPoint, awaitTeleport: false);

            //Task - Button input
            m_DialogueView.EnableButton();
            m_DialogueView.SetButtonText("Click to continue");
            yield return ValidateTask(m_MenuCanvasPosition, null, () => m_ButtonClicked);

            // m_ButtonClicked = false;
            m_DialogueView.SetButtonText("Initiate dialogue");
            yield return ValidateTask(m_MenuCanvasPosition, null, () => m_ButtonClicked);
            m_DialogueView.DisableButton();

            //Task - Dialogue
            m_SampleDialogueTrigger.InitiateDialogue();
            DialogueManager.DialogueEnded += DialogueEnded;
            yield return ValidateTask(m_MenuCanvasPosition, null, () => m_DialogueEnded);

            //Task board


            //Task - Door
            yield return ValidateTask(m_DoorCanvasPosition, m_DoorPoint, () => m_DoorOpened);
            TutorialCompleted?.Invoke(true);
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
                AdvanceTutorial();
            }

            //if (m_State.CurrentNode.ShowPopup) {
            //    if (m_State.CurrentNode.SnapPopupToPosition) {
            //        PopupManager.Instance.SnapPopupToTransform(m_State.CurrentNode.SnapPosition.position
            //            + 0.3f * m_State.CurrentNode.SnapPosition.localScale.magnitude * m_State.CurrentNode.SnapPosition.right);
            //    }

            //    PopupManager.Instance.OpenPopup();
            //}

            yield return new WaitForSeconds(waitDelay);
            yield return new WaitUntil(waitUntil);

            if (teleportPoint != null) {
                TeleportPointManager.DisablePoint(teleportPoint);
            }

            m_EnabledTeleportPoint = null;
            m_TeleportedToEnabledPoint = false;
            m_DialogueView.gameObject.SetActive(false);
            //PopupManager.Instance.ClosePopup();
            //PopupManager.Instance.UnsnapPopup();
        }

        private void CheckActiveTeleportPoint(TeleportPointORST teleportPoint) {
            if (m_EnabledTeleportPoint != null && m_EnabledTeleportPoint == teleportPoint) {
                m_TeleportedToEnabledPoint = true;
                TeleportPointManager.DisablePoint(m_EnabledTeleportPoint);
            }
        }

        private void CheckDoorOpened() {
            m_DoorOpened = true;
        }

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            if (pointerEvent.Type == PointerEventType.Select) {
                m_ObjectPickedUp = true;
            }
        }

        private void ResetTutorial() {
            m_DoorOpened = false;
        }

        private void DialogueEnded(Dialogue dialogue, bool finished) {
            if (dialogue == m_SampleDialogueTrigger.Dialogue) {
                m_DialogueEnded = true;
            }

            Debug.LogError("Dialogue ended");
        }
    }
}
