using ORST.Foundation.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Movement {
    public class RecenterManager : AutoMonoSingleton<RecenterManager> {
        public static void Recenter() {
            Instance.RecenterPosition();
            Instance.RecenterOrientation();
        }

        [SerializeField, Required] private OVRCameraRig m_CameraRig;
        private Transform m_ParentTransform;

        public override bool IsPersistentThroughScenes => false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstanceExists() {
            RecenterManager _ = Instance;
        }

        protected override void OnAwake() {
            base.OnAwake();

            if (m_CameraRig == null) {
                m_CameraRig = FindObjectOfType<OVRCameraRig>();
            }

            m_ParentTransform = m_CameraRig.transform.parent;
        }

        private void Start() {
            // Recenter when the scene loads
            RecenterPosition();
            RecenterOrientation();
        }

        private void OnEnable() {
            AdvancedLocomotionTeleport.Instance.TeleportedToPoint += OnTeleportedToPoint;

            if (OVRManager.display != null) {
                OVRManager.display.RecenteredPose += OnRecenteredPose;
            }
        }

        private void OnDisable() {
            if (AdvancedLocomotionTeleport.Instance != null) {
                AdvancedLocomotionTeleport.Instance.TeleportedToPoint -= OnTeleportedToPoint;
            }

            if (OVRManager.display != null) {
                OVRManager.display.RecenteredPose -= OnRecenteredPose;
            }
        }

        private void RecenterPosition() {
            Vector3 position = -m_CameraRig.centerEyeAnchor.localPosition;
            position.y = 0;
            m_ParentTransform.localPosition = position;
        }

        private void RecenterOrientation() {
            m_ParentTransform.localRotation = Quaternion.Euler(0, -m_CameraRig.centerEyeAnchor.localRotation.eulerAngles.y, 0);
        }

        private void OnTeleportedToPoint(TeleportPointORST _) {
            RecenterPosition();
        }

        private void OnRecenteredPose() {
            // Reset our fake position and rotation when the user re-centers
            // their position using the Oculus button
            m_ParentTransform.localPosition = Vector3.zero;
            m_ParentTransform.localRotation = Quaternion.identity;
        }
    }
}