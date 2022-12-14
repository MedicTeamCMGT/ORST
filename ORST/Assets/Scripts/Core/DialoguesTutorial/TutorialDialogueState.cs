using System;
using System.Linq;
using ORST.Foundation.Extensions;

namespace ORST.Core.Dialogues {
    [Serializable]
    public struct TutorialDialogueState {
        private readonly TutorialDialogue m_Dialogue;
        private int m_CurrentNodeIndex;

        public TutorialDialogueNode CurrentNode => m_Dialogue.OrNull()?.Nodes.ElementAtOrDefault(m_CurrentNodeIndex);
        public bool Started => m_CurrentNodeIndex >= 0;
        public bool Finished => m_CurrentNodeIndex >= (m_Dialogue.OrNull()?.Nodes?.Count ?? -1);
        public int CurrentNodeIndex => m_CurrentNodeIndex;
        public int NodeCount => m_Dialogue.OrNull()?.Nodes?.Count ?? -1;

        public TutorialDialogueState(TutorialDialogue dialogue) {
            m_Dialogue = dialogue;
            m_CurrentNodeIndex = -1;
        }

        public bool Advance() {
            if (Finished) {
                return false;
            }

            m_CurrentNodeIndex++;
            return true;
        }
    }
}
