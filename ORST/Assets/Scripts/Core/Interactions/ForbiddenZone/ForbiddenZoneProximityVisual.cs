using System;
using ORST.Foundation.Core;
using ORST.Foundation.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class ForbiddenZoneProximityVisual : BaseMonoBehaviour {
        private static readonly int s_ColorPropertyId = Shader.PropertyToID("_Color");

        [SerializeField, Required] private ForbiddenZone m_ForbiddenZone;
        [SerializeField, Required] private Renderer m_Renderer;

        private MaterialPropertyBlock m_PropertyBlock;

        private void OnEnable() {
            m_ForbiddenZone.ProximityChanged += OnProximityChanged;
            m_PropertyBlock = new MaterialPropertyBlock();
        }

        private void OnDisable() {
            m_ForbiddenZone.ProximityChanged -= OnProximityChanged;
        }

        private void Start() {
            OnProximityChanged(0.0f);
        }

        private void OnProximityChanged(float proximity) {
            Color materialColor = m_Renderer.material.color.WithA(proximity);
            m_PropertyBlock.SetColor(s_ColorPropertyId, materialColor);
            m_Renderer.SetPropertyBlock(m_PropertyBlock);
        }
    }
}