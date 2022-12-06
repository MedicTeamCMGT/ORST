using Oculus.Interaction;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.LearningModules {
    public class NameTagObject : BaseMonoBehaviour {
        [SerializeField, Required] private Grabbable m_Grabbable;
        [SerializeField, Required] private PointableUnityEventWrapper m_PointableEventWrapper;
        [SerializeField] private NameTagKind m_Kind;

        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;
        private Rigidbody m_Rigidbody;

        public Grabbable Grabbable => m_Grabbable;
        public PointableUnityEventWrapper PointableEventWrapper => m_PointableEventWrapper;
        public Rigidbody Rigidbody => m_Rigidbody;

        private void Awake() {
            m_Rigidbody = m_Grabbable.GetComponent<Rigidbody>();
            m_OriginalPosition = m_Grabbable.transform.position;
            m_OriginalRotation = m_Grabbable.transform.rotation;
        }

        public NameTagKind Kind => m_Kind;

        public void ResetPosition() {
            m_Grabbable.transform.position = m_OriginalPosition;
            m_Grabbable.transform.rotation = m_OriginalRotation;

            if (m_Rigidbody != null) {
                m_Rigidbody.velocity = Vector3.zero;
                m_Rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}