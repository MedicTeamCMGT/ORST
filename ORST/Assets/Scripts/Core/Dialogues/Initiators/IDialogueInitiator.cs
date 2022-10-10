using System;

namespace ORST.Core.Dialogues {
    public interface IDialogueInitiator {
        /// <summary>
        /// Event called when the dialogue is started.
        /// </summary>
        public event Action<Dialogue> DialogueInitiated;
    }
}