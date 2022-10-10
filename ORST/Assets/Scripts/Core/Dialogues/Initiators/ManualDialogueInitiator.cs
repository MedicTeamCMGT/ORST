using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    public class ManualDialogueInitiator : MonoBehaviour, IDialogueInitiator {
        [SerializeField, Required] private Dialogue m_Dialogue;

        public event Action<Dialogue> DialogueInitiated;

        /// <summary>
        /// Manually initiate the dialogue.
        /// </summary>
        public void InitiateDialogue() {
            DialogueInitiated?.Invoke(m_Dialogue);
        }
    }
}