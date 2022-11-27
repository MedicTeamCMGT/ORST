using System;
using System.Collections.Generic;
using System.Threading;
using ORST.Core.Attributes;
using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class ForbiddenZone : BaseMonoBehaviour {
        /// <summary>
        /// Event called when a player enters the forbidden zone
        /// </summary>
        public event Action EnteredZone;

        /// <summary>
        /// Event called when a player exits the forbidden zone
        /// </summary>
        public event Action ExitedZone;

        /// <summary>
        /// Event called when the distance to the forbidden zone changes (only when within the caution distance)
        /// </summary>
        /// <remarks>
        /// Value is in the range [0, 1] where 0 is exactly at caution distance and 1 is inside the forbidden zone
        /// </remarks>
        public event Action<float> ProximityChanged;

        [Title("Forbidden Zone")]
        [SerializeField, Required, SdfIcon(SdfIconType.LightningChargeFill)]
        private Collider m_Collider;
        [SerializeField, Min(0.001f), SdfIcon(SdfIconType.LayersFill)]
        private float m_EnterZoneThreshold = 0.01f;
        [SerializeField, Min(0.001f), SdfIcon(SdfIconType.Record2Fill)]
        [Tooltip("The distance from the forbidden zone that counts as being dangerously close to the zone.")]
        private float m_CautionDistance = 0.25f;

        private List<Transform> m_TrackedTransforms;
        private bool m_InZone;
        private float m_Proximity;

        private void OnEnable() {
            if (FindObjectOfType<OVRCameraRig>() is not {} ovrCameraRig) {
                return;
            }

            (m_TrackedTransforms ??= new List<Transform>(3)).Clear();
            m_TrackedTransforms.Add(ovrCameraRig.centerEyeAnchor);
            m_TrackedTransforms.Add(ovrCameraRig.leftHandAnchor);
            m_TrackedTransforms.Add(ovrCameraRig.rightHandAnchor);
        }

        private void Update() {
            if (m_TrackedTransforms is not { Count: > 0 }) {
                return;
            }

            float minSqrDistance = float.MaxValue;
            bool proximityChanged = false;

            foreach (Transform trackedTransform in m_TrackedTransforms) {
                Vector3 trackedPosition = trackedTransform.position;
                Vector3 closestPoint = m_Collider.ClosestPoint(trackedPosition);

                // NOTE: ClosestPoint returns the same point if the tracked position is inside the collider
                bool insideCollider = (trackedPosition - closestPoint).sqrMagnitude <= m_EnterZoneThreshold * m_EnterZoneThreshold;
                if (insideCollider && !m_InZone) {
                    EnteredZone?.Invoke();
                    m_InZone = true;

                    m_Proximity = 1.0f;
                    ProximityChanged?.Invoke(m_Proximity);
                } else if (!insideCollider && m_InZone) {
                    ExitedZone?.Invoke();
                    m_InZone = false;
                }

                if (m_InZone) {
                    break;
                }

                float sqrDistance = (trackedPosition - closestPoint).sqrMagnitude;
                if (sqrDistance <= m_CautionDistance * m_CautionDistance) {
                    minSqrDistance = Mathf.Min(minSqrDistance, sqrDistance);
                    proximityChanged = true;
                }
            }

            if (!proximityChanged) {
                if (!m_InZone && !Mathf.Approximately(m_Proximity, 0.0f)) {
                    m_Proximity = 0.0f;
                    ProximityChanged?.Invoke(m_Proximity);
                } else if (m_InZone && !Mathf.Approximately(m_Proximity, 1.0f)) {
                    m_Proximity = 1.0f;
                    ProximityChanged?.Invoke(m_Proximity);
                }

                return;
            }

            if (m_InZone) {
                return;
            }

            float proximity = Mathf.Clamp01(1.0f - Mathf.Sqrt(minSqrDistance) / m_CautionDistance);
            if (Mathf.Abs(proximity - m_Proximity) > 0.0001f) {
                m_Proximity = proximity;
                ProximityChanged?.Invoke(m_Proximity);
            }
        }
    }
}