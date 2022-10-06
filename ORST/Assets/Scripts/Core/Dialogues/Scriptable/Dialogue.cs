using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "ORST/Dialogues/Dialogue", order = 0)]
    public class Dialogue : SerializedScriptableObject {
        [SerializeField, Required] private List<DialogueNode> m_Nodes = new();
    }
}