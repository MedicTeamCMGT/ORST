using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace ORST.Core.Interactions
{
    public class InteractableEvents : MonoBehaviour {
        [TitleGroup("Module Task", order: 1000), SerializeField]
        private UnityEvent m_InteractablePickedUp;

        [TitleGroup("Module Task", order: 1000), SerializeField]
        private UnityEvent m_InteractableDropped;

        private void Start() {
            HandGrabInteractable interactable = GetComponentInChildren<HandGrabInteractable>();
            if (interactable != null) {
                interactable.WhenPointerEventRaised += ProcessPointerEvent;
                return;
            }

            throw new NullReferenceException("No HandGrabInteractable found in children");
        }

        private void ProcessPointerEvent(PointerEvent pointerEvent) {
            if (pointerEvent.Type == PointerEventType.Select) {
                m_InteractablePickedUp.Invoke();
            }
            else if (pointerEvent.Type == PointerEventType.Unselect) {
                m_InteractableDropped.Invoke();
            }
        }
    }
}
