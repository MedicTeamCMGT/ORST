using System;
using System.Collections.Generic;
using Oculus.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class PokeInteractableToggleGroup : SerializedMonoBehaviour {
        [SerializeField, Required] private List<PokeInteractableToggle> m_Toggles = new();
        [ShowInInspector, ReadOnly] private int m_SelectedToggle = -1;

        public event Action<PokeInteractableToggle> ToggleSelected;

        private void Awake() {
            for (int i = 0; i < m_Toggles.Count; i++) {
                int toggleIndex = i;
                m_Toggles[toggleIndex].PokeInteractable.WhenStateChanged += args => OnPokeInteractableStateChanged(toggleIndex, args);
            }
        }

        private void OnPokeInteractableStateChanged(int toggleIndex, InteractableStateChangeArgs args) {
            if (args.NewState != InteractableState.Select) {
                return;
            }

            if (toggleIndex == m_SelectedToggle) {
                m_SelectedToggle = -1;
                m_Toggles[toggleIndex].SetSelected(false);
                ToggleSelected?.Invoke(null);
                return;
            }

            if (m_SelectedToggle != -1) {
                m_Toggles[m_SelectedToggle].SetSelected(false);
            }

            m_SelectedToggle = toggleIndex;
            m_Toggles[toggleIndex].SetSelected(true);
            ToggleSelected?.Invoke(m_Toggles[toggleIndex]);
        }
    }
}