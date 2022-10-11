using ORST.Core.Interactions;
using ORST.Foundation.Foundation.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core
{
    [RequireComponent(typeof(MeshRenderer))]
    public class HandHeadProximityMaterial : MonoBehaviour {
        [SerializeField, Required] private HandHeadProximity m_HandHeadProximity;

        private Material m_Material;
        private void Awake() {
            m_Material = GetComponent<MeshRenderer>().material;
        }

        private void OnEnable() {
            //m_HandHeadProximity.ForbiddenSpaceEntered += OnForbiddenSpaceEntered;
            //m_HandHeadProximity.ForbiddenSpaceExited += OnForbiddenSpaceExited;
            m_HandHeadProximity.ForbiddenSpaceUpdated += m_Material.SetMaterialAlpha;
        }

        private void OnDisable() {
            //m_HandHeadProximity.ForbiddenSpaceEntered -= OnForbiddenSpaceEntered;
            //m_HandHeadProximity.ForbiddenSpaceExited -= OnForbiddenSpaceExited;
            m_HandHeadProximity.ForbiddenSpaceUpdated -= m_Material.SetMaterialAlpha;
        }
    }
}
