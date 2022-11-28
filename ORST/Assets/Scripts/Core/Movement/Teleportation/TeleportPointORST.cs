using ORST.Foundation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Movement {
    public class TeleportPointORST : BaseMonoBehaviour, ITeleportPoint {
        [SerializeField, Required] private Transform m_DestinationTransform;
        [SerializeField, Required] private GameObject m_Visuals;

        public Transform DestinationTransform => m_DestinationTransform;
        public GameObject Visuals => m_Visuals;

        private void OnEnable() {
            TeleportPointManager.RegisterPoint(this);
        }

        private void OnDisable() {
            TeleportPointManager.UnregisterPoint(this);
        }
    }
}