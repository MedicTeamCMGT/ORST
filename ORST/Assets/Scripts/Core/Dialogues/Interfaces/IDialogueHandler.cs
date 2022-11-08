namespace ORST.Core.Dialogues {
    public interface IDialogueHandler {
        Dialogue Dialogue { get; }

        void HandleDialogueStarted();
        void HandleDialogueEnded(bool completed);
    }
}