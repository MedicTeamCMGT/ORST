using System;
using System.Collections.Generic;
using Oculus.Interaction;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Attachments {
    public class AttachmentPoint : BaseMonoBehaviour {
        public event Action<AttachableObject> ObjectAttached;
        public event Action<AttachableObject> ObjectDetached;

        [Title("References")]
        [SerializeField, Required] private Collider m_AttachmentCollider;

        private readonly HashSet<AttachableObject> m_PotentialAttachables = new();
        private AttachableObject m_AttachedObject;
        private Transform m_AttachedObjectParent;

        private void InitializeAttachableObject(AttachableObject attachableObject) {
            // This probably means that it's not being grabbed so we can go ahead and immediately attach it
            if (attachableObject.Grabbable.SelectingPointsCount == 0) {
                AttachObject(attachableObject);
                return;
            }

            attachableObject.Grabbed.AddListener(OnAttachableObjectGrabbed);
            attachableObject.Released.AddListener(OnAttachableObjectReleased);
        }

        private void ResetAttachableObject(AttachableObject attachableObject) {
            attachableObject.Grabbed.RemoveListener(OnAttachableObjectGrabbed);
            attachableObject.Released.RemoveListener(OnAttachableObjectReleased);
        }

        private void AttachObject(AttachableObject attachableObject) {
            // idk if this is still needed. maybe it's better to disable attaching if there's already an object attached
            if (m_AttachedObject != null) {
                ResetAttachableObject(m_AttachedObject);
                DetachObject(m_AttachedObject);
                m_AttachedObject = null;
            }

            // Actually attach object
            m_AttachedObject = attachableObject;
            m_AttachedObjectParent = attachableObject.transform.parent;
            m_AttachedObject.Rigidbody.isKinematic = true;
            m_AttachedObject.transform.SetParent(transform);
            m_AttachedObject.transform.SetPose(transform.GetPose());

            // Clear potential attachables
            m_PotentialAttachables.Remove(m_AttachedObject);
            foreach (AttachableObject potentialAttachable in m_PotentialAttachables) {
                ResetAttachableObject(potentialAttachable);
            }
            m_PotentialAttachables.Clear();

            ObjectAttached?.Invoke(m_AttachedObject);
        }

        private void DetachObject(AttachableObject attachableObject) {
            attachableObject.transform.SetParent(m_AttachedObjectParent);
            m_AttachedObject = null;
            ObjectDetached?.Invoke(attachableObject);
        }

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<AttachableObject>() is not { Grabbable: { SelectingPointsCount: > 0 } } attachedObject) {
                // Object is not an attachable or is not being grabbed. We don't allow objects which aren't grabbed
                // because then you might just run into an object accidentally.
                return;
            }

            m_PotentialAttachables.Add(attachedObject);
            InitializeAttachableObject(attachedObject);

        }

        private void OnTriggerExit(Collider other) {
            if (other.GetComponent<AttachableObject>() is not { } attachableObject) {
                return;
            }

            m_PotentialAttachables.Remove(attachableObject);
            ResetAttachableObject(attachableObject);
        }

        private void OnAttachableObjectGrabbed(AttachableObject attachableObject) {
            DetachObject(attachableObject);
            m_PotentialAttachables.Add(attachableObject);
        }

        private void OnAttachableObjectReleased(AttachableObject attachableObject) {
            AttachObject(attachableObject);
        }
    }
}