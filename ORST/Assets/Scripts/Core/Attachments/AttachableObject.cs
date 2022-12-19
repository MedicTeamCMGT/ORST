using Oculus.Interaction;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ORST.Core.Attachments {
    public class AttachableObject : BaseMonoBehaviour {
        [Title("References")]
        [SerializeField, Required] private Rigidbody m_Rigidbody;
        [SerializeField, Required] private Grabbable m_Grabbable;
        [SerializeField, Required] private PointableUnityEventWrapper m_EventWrapper;

        public Rigidbody Rigidbody => m_Rigidbody;
        public Grabbable Grabbable => m_Grabbable;

        public UnityEvent<AttachableObject> Grabbed { get; private set; }
        public UnityEvent<AttachableObject> Released { get; private set; }

        private void Awake() {
            Grabbed = new UnityEvent<AttachableObject>();
            Released = new UnityEvent<AttachableObject>();
            m_EventWrapper.WhenSelect.AddListener(InvokeGrabbed);
            m_EventWrapper.WhenUnselect.AddListener(InvokeReleased);
        }

        private void OnDestroy() {
            m_EventWrapper.WhenSelect.RemoveListener(InvokeGrabbed);
            m_EventWrapper.WhenUnselect.RemoveListener(InvokeReleased);
        }

        private void InvokeGrabbed() => Grabbed?.Invoke(this);
        private void InvokeReleased() => Released?.Invoke(this);
    }
}