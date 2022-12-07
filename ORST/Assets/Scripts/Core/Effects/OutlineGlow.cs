using DG.Tweening;
using EPOOutline;
using ORST.Foundation;
using ORST.Foundation.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Effects {
    public class OutlineGlow : BaseMonoBehaviour {
        [Title("References")]
        [SerializeField, Required] private Outlinable m_Outlinable;

        [Title("Settings")]
        [SerializeField] private GlowType m_GlowType = GlowType.AlphaOnly;
        [SerializeField, ShowIf(nameof(m_GlowType), GlowType.AlphaOnly)] private float m_MinAlpha = 0.75f;
        [SerializeField, ShowIf(nameof(m_GlowType), GlowType.AlphaOnly)] private float m_MaxAlpha = 1.0f;
        [SerializeField, ShowIf(nameof(m_GlowType), GlowType.ColorAndAlpha)] private Color m_MinColor = Color.white;
        [SerializeField, ShowIf(nameof(m_GlowType), GlowType.ColorAndAlpha)] private Color m_MaxColor = Color.white;
        [Space]
        [SerializeField, SuffixLabel("seconds")] private float m_GlowDuration = 1.0f;
        [SerializeField, SuffixLabel("seconds")] private float m_ShowDuration = 0.5f;

        private bool m_IsVisible;
        private bool m_IsGlowing;

        private Tween m_ShowHideTween;
        private Tween m_GlowTween;

        public bool IsVisible => m_IsVisible;
        public bool IsGlowing => m_IsGlowing;

        private void Awake() {
            m_IsVisible = false;
            m_IsGlowing = false;
            m_Outlinable.FrontParameters.Color = m_GlowType == GlowType.AlphaOnly
                ? m_Outlinable.FrontParameters.Color.WithA(0.0f)
                : m_MinColor.WithA(0.0f);
        }

        public Tween Show() {
            if (m_ShowHideTween is { active: true }) {
                m_ShowHideTween.Kill();
            }

            if (m_GlowTween is { active: true }) {
                m_GlowTween.Kill();
            }

            return m_ShowHideTween = m_GlowType is GlowType.AlphaOnly
                ? m_Outlinable.FrontParameters.DOFade(m_MaxAlpha, m_ShowDuration)
                : m_Outlinable.FrontParameters.DOColor(m_MaxColor, m_ShowDuration);
        }

        public Tween Hide() {
            if (m_ShowHideTween is { active: true }) {
                m_ShowHideTween.Kill();
            }

            if (m_GlowTween is { active: true }) {
                m_GlowTween.Kill();
            }

            return m_ShowHideTween = m_GlowType is GlowType.AlphaOnly
                ? m_Outlinable.FrontParameters.DOFade(0.0f, m_ShowDuration)
                : m_Outlinable.FrontParameters.DOColor(m_MinColor.WithA(0.0f), m_ShowDuration);
        }

        public Tween StartGlow() {
            if (m_ShowHideTween is { active: true }) {
                m_ShowHideTween.Kill();
            }

            if (m_GlowTween is { active: true }) {
                m_GlowTween.Kill();
            }

            m_IsGlowing = true;
            return m_GlowTween = m_GlowType is GlowType.AlphaOnly
                ? m_Outlinable.FrontParameters.DOFade(m_MinAlpha, m_GlowDuration).From(m_MaxAlpha).SetLoops(-1, LoopType.Yoyo)
                : m_Outlinable.FrontParameters.DOColor(m_MinColor, m_GlowDuration).From(m_MaxColor).SetLoops(-1, LoopType.Yoyo);
        }

        public Tween StopGlow(bool hide = true) {
            if (m_ShowHideTween is { active: true }) {
                m_ShowHideTween.Kill();
            }

            if (m_GlowTween is { active: true }) {
                m_GlowTween.Kill();
            }

            m_IsGlowing = false;

            if (hide) {
                return m_ShowHideTween = m_GlowType is GlowType.AlphaOnly
                    ? m_Outlinable.FrontParameters.DOFade(0.0f, m_ShowDuration)
                    : m_Outlinable.FrontParameters.DOColor(m_MinColor.WithA(0.0f), m_ShowDuration);
            } else {
                return m_ShowHideTween = m_GlowType is GlowType.AlphaOnly
                    ? m_Outlinable.FrontParameters.DOFade(m_MaxAlpha, m_ShowDuration)
                    : m_Outlinable.FrontParameters.DOColor(m_MaxColor, m_ShowDuration);
            }
        }

        private enum GlowType {
            AlphaOnly,
            ColorAndAlpha
        }
    }
}