using System;
using System.Collections.Generic;
using DG.Tweening;
using Oculus.Interaction;
using ORST.Core.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using Tween = DG.Tweening.Tween;

namespace ORST.Core.Interactions {
    public class PokeInteractableToggleVisual : MonoBehaviour {
        [SerializeField, Required] private PokeInteractableToggle m_PokeInteractableToggle;
        [SerializeField, Required] private Transform m_ButtonBaseTransform;
        [Space]
        [SerializeField, Range(0.0f, 1.0f), SuffixLabel("ratio")] private float m_NormalStateDistance = 0.5f;
        [SerializeField, Range(0.0f, 1.0f), SuffixLabel("ratio")] private float m_SelectedStateDistance = 0.5f;
        [SerializeField, SuffixLabel("seconds")] private float m_StateTransitionDuration = 0.25f;

        private bool m_Started;

        private float m_MaxOffsetAlongNormal;
        private Vector2 m_PlanarOffset;
        private HashSet<PokeInteractor> m_Interactors;

        private bool m_IsSelected;
        private bool m_WasSelected;
        private InteractableState m_CurrentState = InteractableState.Normal;
        private InteractableState m_LastState = InteractableState.Normal;
        private bool m_IsTransitioningToHoverState;
        private bool m_IsTransitioningToNormalState;
        private Tween m_TransitionToHoverTween;
        private Tween m_TransitionToNormalTween;

        private void Start() {
            using StartScopeDisposable scope = this.StartScope(() => m_Started, started => m_Started = started);

            Assert.IsNotNull(m_PokeInteractableToggle);
            Assert.IsNotNull(m_ButtonBaseTransform);

            m_Interactors = new HashSet<PokeInteractor>();

            Vector3 baseTransformPosition = m_ButtonBaseTransform.position;
            Vector3 baseTransformForward = m_ButtonBaseTransform.forward;
            Vector3 transformPosition = transform.position;

            m_MaxOffsetAlongNormal = Vector3.Dot(transformPosition - baseTransformPosition, -1.0f * baseTransformForward);
            Vector3 pointOnPlane = transformPosition - m_MaxOffsetAlongNormal * baseTransformForward;
            m_PlanarOffset = new Vector2(
                Vector3.Dot(pointOnPlane - baseTransformPosition, m_ButtonBaseTransform.right),
                Vector3.Dot(pointOnPlane - baseTransformPosition, m_ButtonBaseTransform.up)
            );
        }

        private void OnEnable() {
            if (!m_Started) {
                return;
            }

            m_Interactors.Clear();
            m_Interactors.UnionWith(m_PokeInteractableToggle.PokeInteractable.Interactors);
            m_PokeInteractableToggle.PokeInteractable.WhenInteractorAdded.Action += OnInteractorAdded;
            m_PokeInteractableToggle.PokeInteractable.WhenInteractorRemoved.Action += OnInteractorRemoved;
            m_PokeInteractableToggle.PokeInteractable.WhenStateChanged += OnInteractableStateChanged;
        }

        private void OnDisable() {
            if (!m_Started) {
                return;
            }

            m_Interactors.Clear();
            m_PokeInteractableToggle.PokeInteractable.WhenInteractorAdded.Action -= OnInteractorAdded;
            m_PokeInteractableToggle.PokeInteractable.WhenInteractorRemoved.Action -= OnInteractorRemoved;
            m_PokeInteractableToggle.PokeInteractable.WhenStateChanged -= OnInteractableStateChanged;
        }

        private void Update() {
            InteractableState state = m_CurrentState;

            // If the button is currently selected force the state to be Hover.
            // This prevents the button from moving back and forth between the Hover and Normal state.
            if (m_IsSelected) {
                state = InteractableState.Hover;
            }

            switch (state) {
                case InteractableState.Normal: {
                    // Stop Hover transition if active
                    if (m_IsTransitioningToHoverState) {
                        if (m_TransitionToHoverTween != null) {
                            m_TransitionToHoverTween.Kill();
                            m_TransitionToHoverTween = null;
                        }

                        m_IsTransitioningToHoverState = false;
                    }

                    // Start normal transition if switching from a different state
                    if (m_LastState is InteractableState.Hover or InteractableState.Select || (m_WasSelected && !m_IsSelected)) {
                        m_IsTransitioningToNormalState = true;
                        if (m_TransitionToNormalTween is { active: true }) {
                            m_TransitionToNormalTween.Kill();
                        }

                        m_TransitionToNormalTween = DOVirtual.Float(
                            1.0f,
                            m_NormalStateDistance,
                            m_StateTransitionDuration,
                            value => transform.position = GetVisualPosition(m_MaxOffsetAlongNormal * value)
                        ).OnComplete(() => {
                            m_TransitionToNormalTween = null;
                            m_IsTransitioningToNormalState = false;
                        });
                    }

                    // Update the normal state
                    if (!m_IsTransitioningToNormalState) {
                        UpdateNormalState();
                    }

                    break;
                }
                case InteractableState.Select:
                case InteractableState.Hover: {
                    // Stop Normal transition if active
                    if (m_IsTransitioningToNormalState) {
                        if (m_TransitionToNormalTween != null) {
                            m_TransitionToNormalTween.Kill();
                            m_TransitionToNormalTween = null;
                        }

                        m_IsTransitioningToNormalState = false;
                    }

                    // Start hover transition if switching from a different state
                    if (m_LastState == InteractableState.Normal) {
                        m_IsTransitioningToHoverState = true;
                        if (m_TransitionToHoverTween is { active: true }) {
                            m_TransitionToHoverTween.Kill();
                        }

                        m_TransitionToHoverTween = DOVirtual.Float(
                            m_NormalStateDistance,
                            1.0f,
                            m_StateTransitionDuration,
                            value => transform.position = GetVisualPosition(m_MaxOffsetAlongNormal * value)
                        ).OnComplete(() => {
                            m_TransitionToHoverTween = null;
                            m_IsTransitioningToHoverState = false;
                        });
                    }

                    // Update the hover state
                    if (!m_IsTransitioningToHoverState) {
                        UpdateHoverState();
                    }

                    break;
                }
                case InteractableState.Disabled: break;
                default: throw new ArgumentOutOfRangeException($"{nameof(InteractableState)}.{state} is not handled", (Exception)null);
            }

            m_LastState = state;
            m_WasSelected = m_IsSelected;
        }

        public void SetSelected(bool selected) {
            m_IsSelected = selected;
        }

        private Vector3 GetVisualPosition(float distanceAlongNormal) {
            return m_ButtonBaseTransform.position +
                   m_ButtonBaseTransform.forward * (-1.0f * distanceAlongNormal) +
                   m_ButtonBaseTransform.right * m_PlanarOffset.x +
                   m_ButtonBaseTransform.up * m_PlanarOffset.y;
        }

        private void UpdateNormalState() {
            // Keep a constant distance from the base transform based on the normal state distance (ratio 0..1 of the max offset)
            transform.position = GetVisualPosition(m_MaxOffsetAlongNormal * m_NormalStateDistance);
        }

        private void UpdateHoverState() {
            // If the button is selected, clamp the position to the selected state distance
            float closestDistance = m_IsSelected ? m_SelectedStateDistance * m_MaxOffsetAlongNormal : m_MaxOffsetAlongNormal;

            // Check each interactor to see if it is closer than the current closest distance
            foreach (PokeInteractor interactor in m_Interactors) {
                float pokeDistance = Vector3.Dot(interactor.Origin - m_ButtonBaseTransform.position, -1.0f * m_ButtonBaseTransform.forward);
                if (pokeDistance < 0.0f) {
                    pokeDistance = 0.0f;
                }

                closestDistance = Mathf.Min(closestDistance, pokeDistance);
            }

            transform.position = GetVisualPosition(closestDistance);
        }

        private void OnInteractorAdded(PokeInteractor interactor) => m_Interactors.Add(interactor);
        private void OnInteractorRemoved(PokeInteractor interactor) => m_Interactors.Remove(interactor);
        private void OnInteractableStateChanged(InteractableStateChangeArgs stateChangeArgs) => m_CurrentState = stateChangeArgs.NewState;
    }
}