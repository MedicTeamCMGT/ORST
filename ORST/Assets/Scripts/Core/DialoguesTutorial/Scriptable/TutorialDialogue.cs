using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [CreateAssetMenu(fileName = "New Tutorial Dialogue", menuName = "ORST/Dialogues/TutorialDialogue", order = 1)]
    public class TutorialDialogue : SerializedScriptableObject {
        [SerializeField, Required] private List<TutorialDialogueNode> m_Nodes = new();

        public List<TutorialDialogueNode> Nodes => m_Nodes;
    }
}