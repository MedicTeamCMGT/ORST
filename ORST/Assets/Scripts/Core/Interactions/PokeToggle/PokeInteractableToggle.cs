using Oculus.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class PokeInteractableToggle : MonoBehaviour {
        [Title("References")]
        [SerializeField, Required] private PokeInteractable m_PokeInteractable;
        [SerializeField, Required] private PokeInteractableToggleVisual m_PokeInteractableVisual;
        [SerializeField, Required] private RoundedBoxProperties m_RoundedBoxProperties;
        [SerializeField, Required] private InteractableColorVisual m_InteractableColorVisual;

        [Title("Settings")]
        [SerializeField] private Color m_SelectedColor;

        public PokeInteractable PokeInteractable => m_PokeInteractable;
        public PokeInteractableToggleVisual PokeInteractableVisual => m_PokeInteractableVisual;
        public RoundedBoxProperties RoundedBoxProperties => m_RoundedBoxProperties;
        public InteractableColorVisual InteractableColorVisual => m_InteractableColorVisual;

        public Color SelectedColor => m_SelectedColor;

        private bool m_IsSelected = false;

        public void SetSelected(bool isSelected) {
            if (isSelected == m_IsSelected) {
                return;
            }

            m_IsSelected = isSelected;
            if (isSelected) {
                SetSelected();
            } else {
                SetUnselected();
            }
        }

        private void SetSelected() {
            m_PokeInteractableVisual.SetSelected(true);
            m_InteractableColorVisual.enabled = false;
            m_RoundedBoxProperties.Color = m_SelectedColor;
        }

        private void SetUnselected() {
            m_PokeInteractableVisual.SetSelected(false);
            m_InteractableColorVisual.enabled = true;
        }
    }
}