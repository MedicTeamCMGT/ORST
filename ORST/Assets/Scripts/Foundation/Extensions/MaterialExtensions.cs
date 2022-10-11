using UnityEngine;

namespace ORST.Foundation.Foundation.Extensions {
    public static class MaterialExtensions {
        public static void SetMaterialAlpha(this Material material, float a) {
            Color currentColor = material.color;
            currentColor.a = a;
            material.color = currentColor;
        }
    }
}