using System.Linq;
using ORST.Foundation.StrongTypes;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace ORST.Foundation.Editor.Drawers {
    public class RadiansDrawer : OdinValueDrawer<Radians> {
        protected override void DrawPropertyLayout(GUIContent label) {
            Property.Children.First().Draw(label);
        }
    }
}