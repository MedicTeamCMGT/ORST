using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "ORST/Dialogues/Dialogue", order = 0)]
    public class Dialogue : SerializedScriptableObject {
        [SerializeField, Required] private List<DialogueNode> m_Nodes = new();

        public ReadOnlyCollection<DialogueNode> Nodes => m_Nodes.AsReadOnly();
    }
}