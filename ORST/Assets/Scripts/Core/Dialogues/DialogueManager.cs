using System;
using System.Collections.Generic;
using System.Linq;
using ORST.Foundation.Singleton;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ORST.Core.Dialogues {
    public class DialogueManager : MonoSingleton<DialogueManager> {
        [OdinSerialize, Required, InlineButton(nameof(FindAllDialogueHandlers), "Find")]
        private List<IDialogueHandler> m_DialogueHandlers;

        private Dictionary<Dialogue, IDialogueHandler> m_DialogueHandlerDictionary;
        private Dialogue m_ActiveDialogue;
        private IDialogueHandler m_ActiveDialogueHandler;

        /// <summary>
        /// Event called when a dialogue is started.
        /// </summary>
        public static event Action<Dialogue> DialogueStarted;

        /// <summary>
        /// Event called when a dialogue has ended.
        /// </summary>
        public static event Action<Dialogue> DialogueEnded;

        /// <summary>
        /// Gets a reference to the active dialogue.
        /// </summary>
        public static Dialogue ActiveDialogue => Instance.m_ActiveDialogue;

        protected override void OnAwake() {
            base.OnAwake();

            m_DialogueHandlerDictionary = new Dictionary<Dialogue, IDialogueHandler>();
            foreach (IDialogueHandler dialogueHandler in m_DialogueHandlers) {
                m_DialogueHandlerDictionary.Add(dialogueHandler.Dialogue, dialogueHandler);
            }
        }

        /// <summary>
        /// Start the given <see cref="Dialogue"/>.
        /// </summary>
        public static void StartDialogue(Dialogue dialogue) {
            if (Instance.m_ActiveDialogue != null) {
                EndDialogue();
            }

            Instance.m_ActiveDialogueHandler = Instance.m_DialogueHandlerDictionary[dialogue];
            Instance.m_ActiveDialogue = dialogue;

            Instance.m_ActiveDialogueHandler.HandleDialogueStarted();
            DialogueStarted?.Invoke(dialogue);
        }

        /// <summary>
        /// End the current dialogue.
        /// </summary>
        public static void EndDialogue() {
            if (Instance.m_ActiveDialogue == null) {
                return;
            }

            Instance.m_ActiveDialogueHandler.HandleDialogueEnded();
            DialogueEnded?.Invoke(Instance.m_ActiveDialogue);

            Instance.m_ActiveDialogue = null;
            Instance.m_ActiveDialogueHandler = null;
        }

        private void FindAllDialogueHandlers() {
            m_DialogueHandlers = new List<IDialogueHandler>(FindObjectsOfType<MonoBehaviour>().OfType<IDialogueHandler>());
        }
    }
}