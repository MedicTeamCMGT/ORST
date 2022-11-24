using System.Collections;
using ORST.Core.Dialogues;
using ORST.Core.Movement;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace ORST.Core.ModuleTasks {
    /// <summary>
    /// A custom implementation of the ORST tutorial.
    /// </summary>
    ///
    public class TeleportTutorialTask : MonoBehaviour, IDialogueHandler {
        public Dialogue Dialogue => m_Dialogue;
        [SerializeField] private Dialogue m_Dialogue;
        [SerializeField, Required] private DialogueNPC m_NPC;
        [SerializeField, Required] private DialogueView m_DialogueViewPrefab;

        [ShowInInspector] private DialogueState m_State;
        [ShowInInspector] private bool m_FinishedInteractingWithUI;
        [ShowInInspector] private DialogueView m_DialogueView;
        
        [Header("Teleportation")]
        [SerializeField] private TeleportInputHandlerHands m_TeleportInputHandlerHands;
        private bool m_TutorialDecisionMade;

        private void Start() {
            DialogueManager.StartDialogue(m_Dialogue);
        }

        public void HandleDialogueStarted() {
            Assert.AreEqual(DialogueManager.ActiveDialogue, Dialogue);

            m_State = new DialogueState(Dialogue);
            Assert.IsTrue(m_State.Advance(), "m_State.Advance() returned false; the dialogue is empty.");

            m_DialogueView = Instantiate(m_DialogueViewPrefab);
            m_DialogueView.gameObject.SetActive(true);
            m_DialogueView.Initialize(m_NPC, OnOptionSelected);
            m_DialogueView.LoadState(m_State.CurrentNode);
        }


        public void HandleDialogueEnded(bool completed) {
            Debug.Log($"Dialogue ended. Completed: {completed}");
            gameObject.SetActive(false);
        }

        private void OnOptionSelected(int optionIndex) {
            Debug.Log($"Option {optionIndex} selected.");
            m_DialogueView.LoadState(m_State.CurrentNode);

            //If the user has selected the last option, we can end the dialogue.
            if (m_State.CurrentNodeIndex == 0) {
                if (optionIndex == 1) {
                    //Close tutorial
                    TerminateDialogue();
                }

                StartCoroutine(Tutorial());
            }

            if (m_State.CurrentNodeIndex == m_State.NodeCount - 1) {
                m_FinishedInteractingWithUI = true;
                TerminateDialogue();
            }
        }

        private void TerminateDialogue() {
            Destroy(m_DialogueView.gameObject);
            m_DialogueView = null;
            DialogueManager.EndDialogue(m_FinishedInteractingWithUI);
            m_FinishedInteractingWithUI = false;
            m_State = default;
        }

        private IEnumerator Tutorial() {
            //Dialogue
            yield return new WaitUntil(() => m_TeleportInputHandlerHands.GetIntention() == LocomotionTeleport.TeleportIntentions.Aim);
            //Check for teleportation
           // yield return new WaitUntil();
            //Dialogue
            //Check for object interaction
            //Dialogue
        }
    }
}
