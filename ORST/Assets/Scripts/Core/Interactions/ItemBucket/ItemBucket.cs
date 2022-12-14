using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ORST.Core {
    [RequireComponent(typeof(BoxCollider))]
    public class ItemBucket : MonoBehaviour {
        public Action ItemBucketFilled;

        [SerializeField] private List<GameObject> m_Items;

        private void OnTriggerEnter(Collider collider) {
            if (!collider.CompareTag("Interactable") || !m_Items.Contains(collider.gameObject)) {
                return;
            }

            HandGrabInteractable interactable = collider.GetComponentInChildren<HandGrabInteractable>();

            if (interactable == null) {
                return;
            }

            //If it's currently still in the user's hand, hook the remove function up to the pointer event and check for unselect.
            //Otherwise; remove item immediately.
            if (interactable.State == InteractableState.Select) {
                interactable.WhenPointerEventRaised += pointerEvent => {
                    if (pointerEvent.Type == PointerEventType.Unselect) {
                        RemoveItem(collider.gameObject);
                    }
                };

                return;
            }

            RemoveItem(collider.gameObject);
        }

        private void RemoveItem(GameObject gameObject) {
            m_Items.Remove(gameObject);
            if (m_Items.Count == 0) {
                ItemBucketFilled?.Invoke();
            }
        }
    }
}
