using System.Collections.Generic;
using UnityEngine;

namespace ORST.Core.Movement {
    public class TeleportAimLaserVisualBezier : TeleportSupport {
        [SerializeField] private LineRenderer m_LaserPrefab;
        [SerializeField, Min(3)] private int m_Resolution = 500;

        private LineRenderer m_LineRenderer;

        private void Awake() {
            m_LaserPrefab.gameObject.SetActive(false);
            m_LineRenderer = Instantiate(m_LaserPrefab);
        }

        protected override void AddEventHandlers() {
            base.AddEventHandlers();
            LocomotionTeleport.EnterStateAim += EnterAimState;
            LocomotionTeleport.ExitStateAim += ExitAimState;
            LocomotionTeleport.UpdateAimData += UpdateAimData;
        }

        protected override void RemoveEventHandlers() {
            base.RemoveEventHandlers();
            LocomotionTeleport.EnterStateAim -= EnterAimState;
            LocomotionTeleport.ExitStateAim -= ExitAimState;
            LocomotionTeleport.UpdateAimData -= UpdateAimData;
        }

        private void EnterAimState() {
            m_LineRenderer.gameObject.SetActive(true);
        }

        private void ExitAimState() {
            m_LineRenderer.gameObject.SetActive(false);
        }

        private void UpdateAimData(LocomotionTeleport.AimData obj) {
            m_LineRenderer.sharedMaterial.color = Color.green;

            //Note: The line renderer is dis-/enabled in the Enter/ExitAimState methods.
            //   This dis-/enables it based on whether the user is hitting a destination.
            if (LocomotionTeleport.CurrentIntention != LocomotionTeleport.TeleportIntentions.Aim || !obj.TargetValid) {
                m_LineRenderer.gameObject.SetActive(false);
                return;
            }

            m_LineRenderer.gameObject.SetActive(true);

            List<Vector3> points = obj.Points;
            m_LineRenderer.positionCount = m_Resolution;
            Vector3 firstElement = points[0];
            Vector3 lastElement = points[^1];
            LocomotionTeleport.InputHandler.GetAimData(out Ray aimRay);

            for (int i = 0; i < m_Resolution; i++) {
                float t = i / (float)(m_Resolution - 1);
                m_LineRenderer.SetPosition(i, GetPoint(firstElement, firstElement + aimRay.direction * ((lastElement - firstElement) / 2).magnitude, lastElement, t));
            }
        }

        private static Vector3 GetPoint(Vector3 start, Vector3 control, Vector3 end, float t) {
            float t0 = (1.0f - t) * (1.0f - t);
            float t1 = t * t;
            return control + t0 * (start - control) + t1 * (end - control);
        }
    }
}
