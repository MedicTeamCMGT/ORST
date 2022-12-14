using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;

namespace ORST.Core.Movement {
    public class AdvancedTeleportTargetHandlerNode : TeleportTargetHandlerNode {
        [SerializeField, Required, ShowIf(nameof(m_ShowInvalidPositionIndicator))] private GameObject m_InvalidTeleportPosIndicator;
        [SerializeField] private bool m_ShowInvalidPositionIndicator = true;
        private bool m_IsIntersectChanged;
        private bool m_ValidCollisionOnSegment;

        public TeleportPointORST TargetPoint { get; private set; }
        public new LocomotionTeleport.AimData AimData => base.AimData;

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

                TargetPoint = null;

                // Start the testing with the character's current position to the aiming origin to ensure they 
                // haven't just stuck their hand through something that should have prevented movement.
                //
                // The first test won't be added to the aim data results because the visual effects should be from
                // the aiming origin.

                Vector3 current = LocomotionTeleport.transform.position;

                // Enumerate through all the line segments provided by the aim handler, checking for a valid target on each segment,
                // stopping at the first valid target or when the enumerable runs out of line segments.
                AimPoints.Clear();
                LocomotionTeleport.AimHandler.GetPoints(AimPoints);


                m_ValidCollisionOnSegment = false;
                Vector3 segmentColliderPoint = Vector3.zero;
                for (int i = 0; i < AimPoints.Count; i++) {
                    Vector3 adjustedPoint = AimPoints[i];
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
                        break;
                    }

                    current = AimPoints[i];
                }

                //@Maurice
                if (m_ShowInvalidPositionIndicator &&
                    LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim) {
                    m_InvalidTeleportPosIndicator.SetActive(m_ValidCollisionOnSegment);
                    if (m_ValidCollisionOnSegment) {
                        m_InvalidTeleportPosIndicator.transform.position = segmentColliderPoint;
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

        /// <summary>
        /// This method will be called while the LocmotionTeleport component is in the aiming state, once for each
        /// line segment that the targeting beam requires.
        /// The function should return true whenever an actual target location has been selected.
        /// </summary>
        protected override bool ConsiderTeleport(Vector3 start, ref Vector3 end) {
            // If the ray hits the world, consider it valid and update the aimRay to the end point.
            if (!LocomotionTeleport.AimCollisionTest(start, end, AimCollisionLayerMask | TeleportLayerMask, out AimData.TargetHitInfo)) {
                return false;
            }

            if (AimData.TargetHitInfo.collider.gameObject.GetComponent<TeleportPointORST>() is not { } tp
             || !TeleportPointManager.IsAvailable(tp)) {
                return false;
            }

            // The targeting test discovered a valid teleport node. Now test to make sure there is line of sight to the 
            // actual destination. Since the teleport destination is expected to be right on the ground, use the LOSOffset 
            // to bump the collision check up off the ground a bit.
            Vector3 destination = tp.DestinationTransform.position;
            Vector3 offsetEnd = new(destination.x, destination.y + LOSOffset, destination.z);

            if (LocomotionTeleport.AimCollisionTest(start, offsetEnd, AimCollisionLayerMask & ~TeleportLayerMask, out AimData.TargetHitInfo)) {
                return false;
            }

            end = destination;
            TargetPoint = tp;
            return true;
        }

        private void TargetAimExit() {
            if (LocomotionTeleport.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim) {
                return;
            }

            m_IsIntersectChanged = m_ValidCollisionOnSegment;
            m_InvalidTeleportPosIndicator.SetActive(false);
        }
    }
}