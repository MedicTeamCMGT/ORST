using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Attributes {
    public class SdfIconAttribute : Attribute {
        public SdfIconType Icon { get; set; }

        public float Padding { get; set; } = 1.0f;
        public float Margin { get; set; } = -2.0f;
        public SdfIconAlignment Alignment { get; set; } = SdfIconAlignment.Left;

#if UNITY_EDITOR
        public string Color { get; set; } = UnityEditor.EditorGUIUtility.isProSkin ? "#FFFFFF" : "#2F2F2F";
#else
        public string Color { get; set; }
#endif
        public Color GetColor() => ColorUtility.TryParseHtmlString(this.Color, out Color color) ? color : UnityEngine.Color.white;

        public SdfIconAttribute(SdfIconType icon) {
            this.Icon = icon;
        }
    }

    public enum SdfIconAlignment {
        Left,
        Right,
    }
}