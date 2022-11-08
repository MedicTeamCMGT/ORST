using Oculus.Interaction;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace ORST.Core.Dialogues.Demo {
    public class SampleDialogueHandler : BaseMonoBehaviour, IDialogueHandler {
        [field: SerializeField] public Dialogue Dialogue { get; private set; }
        [SerializeField, Required] private DialogueNPC m_NPC;
        [SerializeField, Required] private PointableUnityEventWrapper m_Button;
        [SerializeField, Required] private DialogueView m_DialogueViewPrefab;

        [ShowInInspector] private DialogueState m_State;
        [ShowInInspector] private bool m_FinishedInteractingWithUI;
        [ShowInInspector] private DialogueView m_DialogueView;

        private void Awake() {
            m_Button.WhenRelease.AddListener(OnButtonClicked);
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

            Assert.IsTrue(m_State.Advance(), "m_State.Advance() returned false; this shouldn't happen with the sample dialogue.");

            m_DialogueView.LoadState(m_State.CurrentNode);

            if (m_State.CurrentNodeIndex == m_State.NodeCount - 1) {
                m_FinishedInteractingWithUI = true;
            }
        }

        private void OnButtonClicked() {
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