using UnityEngine;

namespace ORST.Foundation.Foundation.Extensions {
    public static class ColorExtensions {
        public static void SetMaterialAlpha(this Material material, float a) {
            Color currentColor = material.color;
            currentColor.a = a;
            material.color = currentColor;
        }
    }
}