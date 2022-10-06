using System.Collections.Generic;
using Oculus.Interaction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class RayInteractorCouldHaveCandidate : MonoBehaviour, IActiveState {
        [SerializeField, Required] private RayInteractor m_Interactor;

        [ShowInInspector] public bool Active => CouldHaveCandidate();

        private bool CouldHaveCandidate() {
            if (m_Interactor == null) {
                return false;
            }

            if (m_Interactor.HasCandidate) {
                return true;
            }

            IEnumerable<RayInteractable> interactables = RayInteractable.Registry.List(m_Interactor);
            foreach (RayInteractable interactable in interactables) {
                if (!interactable.Raycast(m_Interactor.Ray, out _, m_Interactor.MaxRayLength, false)) {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}