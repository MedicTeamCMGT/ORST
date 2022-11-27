using ORST.Core.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ORST.Core.Editor.AttributeDrawers {
    public class SdfIconAttributeDrawer : OdinAttributeDrawer<SdfIconAttribute> {
        protected override void DrawPropertyLayout(GUIContent label) {
            GUILayout.BeginHorizontal();

            if (Attribute.Alignment is SdfIconAlignment.Left) {
                float currentLabelWidth = GUIHelper.BetterLabelWidth;
                GUIHelper.BetterLabelWidth = currentLabelWidth - CalculateIconWidth();

                DrawIcon();
                GUILayout.Space(Attribute.Margin);
                CallNextDrawer(label);

                GUIHelper.BetterLabelWidth = currentLabelWidth;
            } else {
                CallNextDrawer(label);
                GUILayout.Space(Attribute.Margin);
                DrawIcon();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(2.0f);
        }

        private void DrawIcon() {
            float padding = Mathf.Max(0.0f, Attribute.Padding);
            float maxSize = EditorGUIUtility.singleLineHeight;
            float size = Mathf.Max(8.0f, maxSize - 2.0f * padding);

            // Get Rect for the icon
            Rect iconRect = GUILayoutUtility.GetRect(maxSize, maxSize, GUILayout.ExpandWidth(false));
            iconRect.width = size;
            iconRect.height = size;
            iconRect.x += padding;
            iconRect.y += padding + (maxSize - size) / 2.0f;

            // Draw the icon
            SdfIcons.DrawIcon(iconRect, Attribute.Icon, Attribute.GetColor());
        }

        private float CalculateIconWidth() {
            float padding = Mathf.Max(0.0f, Attribute.Padding);
            float maxSize = EditorGUIUtility.singleLineHeight;

            float size = Mathf.Max(8.0f, maxSize - 2.0f * padding);
            return size + padding * 2.0f + Attribute.Margin;
        }
    }
}