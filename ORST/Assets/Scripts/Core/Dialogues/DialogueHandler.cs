using Oculus.Interaction;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace ORST.Core.Dialogues {
    public class DialogueHandler : BaseMonoBehaviour, IDialogueHandler {
        [field: SerializeField] public Dialogue Dialogue { get; private set; }
        [SerializeField, Required] private DialogueNPC m_NPC;
        [SerializeField] private PointableUnityEventWrapper m_Button;
        [SerializeField] private bool m_CloseDialogueOnCompletion;
        [SerializeField, Required] private DialogueView m_DialogueViewPrefab;

        private DialogueState m_State;
        private bool m_FinishedInteractingWithUI;
        private DialogueView m_DialogueView;

        private void Awake() {
            if (m_Button != null) {
                m_Button.WhenRelease.AddListener(CloseAndResetDialogue);
            }
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

        private void OnOptionSelected(int optionIndex) {
            Debug.Log($"Option {optionIndex} selected.");

            m_State.Advance();
            if (m_State.CurrentNodeIndex == m_State.NodeCount) {
                m_FinishedInteractingWithUI = true;
                if (m_CloseDialogueOnCompletion) {
                    CloseAndResetDialogue();
                    return;
                }
            }

            m_DialogueView.LoadState(m_State.CurrentNode);
        }

        private void CloseAndResetDialogue() {
            if (!m_FinishedInteractingWithUI) {
                return;
            }

            Destroy(m_DialogueView.gameObject);
            m_DialogueView = null;
            DialogueManager.EndDialogue(true);

            m_FinishedInteractingWithUI = false;
            m_State = default;
        }

        public void HandleDialogueEnded(bool completed) {
            Debug.Log($"Dialogue ended. Completed: {completed}");
        }
    }
}
