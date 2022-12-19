using ORST.Core.Attachments;
using ORST.Core.Effects;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.LearningModules {
    public class NameTagObject : BaseMonoBehaviour {
        [SerializeField, Required] private AttachableObject m_AttachableObject;
        [SerializeField, Required] private OutlineGlow m_OutlineGlow;
        [SerializeField] private NameTagKind m_Kind;

        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;

        public AttachableObject AttachableObject => m_AttachableObject;
        public OutlineGlow OutlineGlow => m_OutlineGlow;
        public NameTagKind Kind => m_Kind;
    }
}