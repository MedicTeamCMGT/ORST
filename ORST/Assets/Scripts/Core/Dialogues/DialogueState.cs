using System;
using System.Linq;
using ORST.Foundation.Extensions;
using Sirenix.OdinInspector;

namespace ORST.Core.Dialogues {
    [Serializable]
    public struct DialogueState {
        [ShowInInspector] private readonly Dialogue m_Dialogue;
        [ShowInInspector] private int m_CurrentNodeIndex;

        [ShowInInspector, ShowIf("@" + nameof(Started) + " && !" + nameof(Finished))]
        public DialogueNode CurrentNode => m_Dialogue.OrNull()?.Nodes.ElementAtOrDefault(m_CurrentNodeIndex);

        [ShowInInspector, ReadOnly] public bool Started => m_CurrentNodeIndex >= 0;
        [ShowInInspector, ReadOnly] public bool Finished => m_CurrentNodeIndex >= (m_Dialogue.OrNull()?.Nodes?.Count ?? -1);
        [ShowInInspector, ReadOnly] public int CurrentNodeIndex => m_CurrentNodeIndex;
        [ShowInInspector, ReadOnly] public int NodeCount => m_Dialogue.OrNull()?.Nodes?.Count ?? -1;

        public DialogueState(Dialogue dialogue) {
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