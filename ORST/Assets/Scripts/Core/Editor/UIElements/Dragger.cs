using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ORST.Core.Editor.UIElements {
    public class Dragger : VisualElement {
        private bool m_Active;
        private Vector2 m_Start;
        private MouseButton m_ActivateButton = MouseButton.LeftMouse;

        public event Action DragStarted;
        public event Action<Vector2, Vector2> DragUpdated;
        public event Action DragStopped;

        public MouseButton ActivateButton {
            get => m_ActivateButton;
            set => m_ActivateButton = value;
        }

        public Dragger() {
            m_Active = false;
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        private void OnMouseDown(MouseDownEvent e) {
            if (m_Active) {
                e.StopImmediatePropagation();
            } else {
                if ((e.target is VisualElement target ? target.panel : null).GetCapturingElement(PointerId.mousePointerId) != null || (MouseButton)e.button != m_ActivateButton) {
                    return;
                }
                m_Start = this.ChangeCoordinatesTo(parent, e.localMousePosition);
                DragStarted?.Invoke();
                m_Active = true;
                this.CaptureMouse();
                e.StopPropagation();
            }
        }

        private void OnMouseUp(MouseUpEvent e) {
            if (!m_Active || (MouseButton)e.button != m_ActivateButton)
                return;
            m_Active = false;
            DragStopped?.Invoke();
            this.ReleaseMouse();
            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e) {
            if (!m_Active)
                return;

            Vector2 current = this.ChangeCoordinatesTo(parent, e.localMousePosition);
            DragUpdated?.Invoke(m_Start, current);

            e.StopPropagation();
        }

        public new class UxmlFactory : UxmlFactory<Dragger, UxmlTraits> {
        }
    }
}