using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ORST.Core.Dialogues {
    public class DialogueView : BaseMonoBehaviour {
        [SerializeField, Required] private TextMeshProUGUI m_NPCName;
        [SerializeField, Required] private TextMeshProUGUI m_NPCTitle;
        [SerializeField, Required] private TextMeshProUGUI m_Text;
        [SerializeField, Required] private List<DialogueButton> m_Buttons;

        [Title("Incorrect Answer")]
        [SerializeField, Required] private GameObject m_IncorrectAnswerContainer;
        [SerializeField, Required] private Button m_IncorrectContinueButton;
        [SerializeField, Required] private TextMeshProUGUI m_IncorrectHint;

        private DialogueNPC m_NPC;
        private Action<int> m_OptionSelectedCallback;
        private DialogueNode m_CurrentNode;

        private void Awake() {
            int i = 0;
            foreach (DialogueButton button in m_Buttons) {
                button.Button.onClick.RemoveAllListeners();
                int index = i;
                button.Button.onClick.AddListener(() => OnOptionSelected(index));
                i++;
            }

            m_IncorrectAnswerContainer.gameObject.SetActive(false);
        }

        public void Initialize(DialogueNPC npc, [NotNull] Action<int> optionSelectedCallback) {
            m_NPC = npc;
            m_OptionSelectedCallback = optionSelectedCallback ?? throw new ArgumentNullException(nameof(optionSelectedCallback));

            m_NPCName.text = npc.Name;
            m_NPCTitle.text = $"<color=#00ff00>Role:</color> <b>{npc.Role}</b>";
        }

        public void LoadState(DialogueNode node) {
            m_CurrentNode = node;
            m_Text.text = node.Text;

            int index = 0;
            foreach (DialogueOption dialogueOption in node.Options) {
                m_Buttons[index].gameObject.SetActive(true);
                m_Buttons[index].Text.text = dialogueOption.Text;

                index++;
            }

            for (int i = index; i < m_Buttons.Count; i++) {
                m_Buttons[i].gameObject.SetActive(false);
            }
        }

        private void OnOptionSelected(int index) {
            DialogueOption selectedOption = m_CurrentNode.Options[index];
            if (selectedOption.IsCorrect) {
                m_OptionSelectedCallback(index);
                return;
            }

            m_IncorrectAnswerContainer.SetActive(true);
            m_IncorrectHint.text = selectedOption.Feedback;
            m_IncorrectContinueButton.onClick.RemoveAllListeners();
            m_IncorrectContinueButton.onClick.AddListener(() => {
                m_IncorrectAnswerContainer.SetActive(false);
                m_OptionSelectedCallback(index);
            });
        }
    }
}