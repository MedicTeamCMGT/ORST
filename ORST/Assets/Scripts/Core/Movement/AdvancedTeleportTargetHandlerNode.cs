using UnityEngine;
using System.Collections;

namespace ORST.Core {
    public class AdvancedTeleportTargetHandlerNode : TeleportTargetHandlerNode {

        [SerializeField] private GameObject invalidTeleportPosIndicator;
        [SerializeField] private bool showInvalidPositionIndicator = true;
        private bool m_IsIntersectChanged;
        private bool m_ValidCollisionOnSegment;

        protected override void OnEnable() {
            base.OnEnable();
            if (LocomotionTeleport is AdvancedLocomotionTeleport teleport) {
                teleport.TargetHandler = this;
            }
        }

        protected override void AddEventHandlers() {
            base.AddEventHandlers();
            LocomotionTeleport.ExitStateAim += TargetAimExit;
        }

        protected override void RemoveEventHandlers() {
            base.RemoveEventHandlers();
            LocomotionTeleport.ExitStateAim -= TargetAimExit;
        }

        protected override IEnumerator TargetAimCoroutine() {
            // While the teleport system is in the aim state, perform the aim logic and consider teleporting.
            while (LocomotionTeleport.CurrentState == LocomotionTeleport.States.Aim) {
                // With each targeting test, we need to reset the AimData to clear the point list and reset flags.
                ResetAimData();

                // Start the testing with the character's current position to the aiming origin to ensure they 
                // haven't just stuck their hand through something that should have prevented movement.
                //
                // The first test won't be added to the aim data results because the visual effects should be from
                // the aiming origin.

                var current = LocomotionTeleport.transform.position;

                // Enumerate through all the line segments provided by the aim handler, checking for a valid target on each segment,
                // stopping at the first valid target or when the enumerable runs out of line segments.
                AimPoints.Clear();
                LocomotionTeleport.AimHandler.GetPoints(AimPoints);

                m_ValidCollisionOnSegment = false;
                Vector3 segmentColliderPoint = Vector3.zero;
                for (int i = 0; i < AimPoints.Count; i++) {
                    var adjustedPoint = AimPoints[i];
                    AimData.TargetValid = ConsiderTeleport(current, ref adjustedPoint);
                    AimData.Points.Add(adjustedPoint);

                    if (AimData.TargetValid) { //This will only be true if it hit an actual teleport point
                        AimData.Destination = ConsiderDestination(adjustedPoint);
                        AimData.TargetValid = AimData.Destination.HasValue;
                        break;
                    }

                    if (!m_ValidCollisionOnSegment && AimData.TargetHitInfo.collider != null) {
                        m_ValidCollisionOnSegment = true;
                        segmentColliderPoint = AimData.TargetHitInfo.point;
                    }

                    current = AimPoints[i];
                }

                //@Maurice
                if (showInvalidPositionIndicator &&
                    LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim) {
                    invalidTeleportPosIndicator.SetActive(m_ValidCollisionOnSegment);
                    if (m_ValidCollisionOnSegment) {
                        invalidTeleportPosIndicator.transform.position = segmentColliderPoint;
                    }

                    if (m_IsIntersectChanged != m_ValidCollisionOnSegment) {
                        m_IsIntersectChanged = m_ValidCollisionOnSegment;
                        AdvancedLocomotionTeleport advLocoTeleport = LocomotionTeleport as AdvancedLocomotionTeleport;
                        if (advLocoTeleport != null) {
                            if (m_ValidCollisionOnSegment) {
                                advLocoTeleport.InvokeOnIntersectEnter();
                            } else {
                                advLocoTeleport.InvokeOnIntersectExit();
                            }
                        }
                    }
                }

                LocomotionTeleport.OnUpdateAimData(AimData);
                yield return null;
            }
        }

        private void TargetAimExit() {
            if (LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim) {
                return;
            }
            m_IsIntersectChanged = m_ValidCollisionOnSegment;
            invalidTeleportPosIndicator.SetActive(false);
        }
    }
}
