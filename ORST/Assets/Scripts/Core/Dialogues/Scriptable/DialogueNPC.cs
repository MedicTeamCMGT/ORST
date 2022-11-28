using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Dialogues {
    [CreateAssetMenu(fileName = "New NPC", menuName = "ORST/Dialogues/NPC", order = 0)]
    public class DialogueNPC : SerializedScriptableObject {
        [SerializeField, LabelText("Name"), Required] private string m_NPCName = "John Doe";
        [SerializeField, Required] private string m_Role = "CEO of John Doe Inc.";
        [SerializeField] private Sprite m_Icon;
        [Space]
        [SerializeField, LabelText("ID"), DisplayAsString] private Guid m_Identifier;

        /// <summary>
        /// Gets a value representing the name of the NPC.
        /// </summary>
        public string Name => m_NPCName;

        /// <summary>
        /// Gets a value representing the role of the NPC.
        /// </summary>
        public string Role => m_Role;

        /// <summary>
        /// Gets a value representing the icon of the NPC. (Optional)
        /// </summary>
        public Sprite Icon => m_Icon;

        /// <summary>
        /// Gets a value representing the unique identifier of the NPC. (Read Only)
        /// </summary>
        public Guid Identifier => m_Identifier;

        private void Reset() {
            m_Identifier = Guid.NewGuid();
        }
    }
}