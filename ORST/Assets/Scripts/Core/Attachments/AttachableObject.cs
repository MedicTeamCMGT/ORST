using System;
using Oculus.Interaction;
using ORST.Core.Utilities;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ORST.Core.Attachments {
    public class AttachableObject : BaseMonoBehaviour {
        [Title("References")]
        [SerializeField, Required] private Rigidbody m_Rigidbody;
        [SerializeField, Required] private Grabbable m_Grabbable;

        public Rigidbody Rigidbody => m_Rigidbody;
        public Grabbable Grabbable => m_Grabbable;

        public UnityEvent<AttachableObject> Grabbed { get; } = new();
        public UnityEvent<AttachableObject> Released { get; } = new();

        private void OnEnable() {
            // Wait a frame so that we subscribe after PhysicsGrabbable
            StartCoroutine(Coroutines.WaitFramesAndThen(1, () => {
                Grabbable.WhenPointerEventRaised += OnPointerEventRaised;
            }));
        }

        private void OnDisable() {
            Grabbable.WhenPointerEventRaised -= OnPointerEventRaised;
        }

        private void OnPointerEventRaised(PointerEvent pointerEvent) {
            if (pointerEvent.Type == PointerEventType.Select) {
                Grabbed.Invoke(this);
            } else if (pointerEvent.Type == PointerEventType.Unselect) {
                Released.Invoke(this);
            }
        }
    }
}