using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Foundation.Core;
using ORST.Foundation.Extensions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class ActiveStateFilter : BaseMonoBehaviour, IActiveState {
        public bool Active => IsActive();

        [OdinSerialize, Required, ShowIf("@(" + nameof(m_Filters) + " & Filters.Grabbing) == Filters.Grabbing")]
        private IHandGrabStateProvider m_GrabState;
        [OdinSerialize, Required, ShowIf("@(" + nameof(m_Filters) + " & Filters.Teleporting) == Filters.Teleporting")]
        private TeleportInputHandler m_TeleportInputHandler;
        [OdinSerialize, Required, ShowIf("@(" + nameof(m_Filters) + " & Filters.RayCanHaveCandidate) == Filters.RayCanHaveCandidate")]
        private IRayInteractorProvider m_RayInteractor;
        [Space]
        [SerializeField]
        private Operation m_Operation;
        [SerializeField, OnValueChanged(nameof(UpdateFilters))]
        private Filters m_Filters;
        [ListDrawerSettings(DraggableItems = false, Expanded = true, IsReadOnly = true, ShowIndexLabels = false, ShowItemCount = false)]
        [OdinSerialize, HideLabel]
        private FilterSettings[] m_FilterSettings;

        private void UpdateFilters() {
            List<Filters> filters = Enum.GetValues(typeof(Filters)).Cast<Filters>().Where(filter => (m_Filters & filter) == filter).ToList();

            // Update m_FilterSettings to match the current filters. Remove any that are no longer valid and add any that are missing.
            FilterSettings[] newFilterSettings = new FilterSettings[filters.Count];

            for (int i = 0; i < filters.Count; i++) {
                FilterSettings filterSetting;
                if (m_FilterSettings != null) {
                    filterSetting = m_FilterSettings.FirstOrDefault(setting => setting.Filter == filters[i]);
                    if (filterSetting.Filter == 0) {
                        filterSetting = new FilterSettings {Filter = filters[i]};
                    }
                } else {
                    filterSetting = new FilterSettings {Filter = filters[i]};
                }

                newFilterSettings[i] = filterSetting;
            }

            m_FilterSettings = newFilterSettings;
        }

        private bool IsActive() {
            return m_Operation switch {
                Operation.And => m_FilterSettings.All(filter => EvaluateFilter(filter.Filter, filter.Invert)),
                Operation.Or => m_FilterSettings.Any(filter => EvaluateFilter(filter.Filter, filter.Invert)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private bool EvaluateFilter(Filters filter, bool invert) {
            return filter switch {
                Filters.Grabbing => invert ^ (m_GrabState?.Value?.IsGrabbing ?? false),
                Filters.RayCanHaveCandidate => invert ^ EvaluateRayCanHaveCandidate(),
                Filters.Teleporting => invert ^ (m_TeleportInputHandler.OrNull()?.GetIntention() != LocomotionTeleport.TeleportIntentions.None),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
            };
        }

        private bool EvaluateRayCanHaveCandidate() {
            if (m_RayInteractor == null) {
                return false;
            }

            if (m_RayInteractor.Value.HasCandidate) {
                return true;
            }

            IEnumerable<RayInteractable> interactables = RayInteractable.Registry.List(m_RayInteractor.Value);
            foreach (RayInteractable interactable in interactables) {
                if (!interactable.Raycast(m_RayInteractor.Value.Ray, out _, m_RayInteractor.Value.MaxRayLength, false)) {
                    continue;
                }

                return true;
            }

            return false;
        }

        [Serializable]
        public struct FilterSettings {
            [field: OdinSerialize, ReadOnly, HorizontalGroup, HideLabel]
            public Filters Filter { get; init; }

            [field: OdinSerialize, HorizontalGroup, LabelWidth(40.0f)]
            public bool Invert { get; init; }
        }

        [Flags]
        public enum Filters {
            Grabbing = 1 << 0,
            RayCanHaveCandidate = 1 << 1,
            Teleporting = 1 << 2,
        }

        private enum Operation { And, Or }
    }
}