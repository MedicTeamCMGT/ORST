using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ORST.Foundation.Extensions;
using QOutline;
using UnityEngine;

namespace ORST.Core.Gfx {
    public static class OutlineDOTweenExtensions {
        public static TweenerCore<Color, Color, ColorOptions> DOColor(this QuickOutline outline, Color endValue, float duration) {
            return DOTween.To(() => outline.OutlineColor, x => outline.OutlineColor = x, endValue, duration).SetTarget(outline);
        }

        public static TweenerCore<float, float, FloatOptions> DOFade(this QuickOutline outline, float endValue, float duration) {
            return DOTween.To(() => outline.OutlineColor.a, x => outline.OutlineColor = outline.OutlineColor.WithA(x), endValue, duration).SetTarget(outline);
        }
    }
}