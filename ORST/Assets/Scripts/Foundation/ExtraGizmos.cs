using System;
using System.Collections.Generic;
using ORST.Foundation.StrongTypes;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ORST.Foundation {
    public static partial class ExtraGizmos {
        private static readonly Stack<Matrix4x4> s_MatrixStack = new();
        private static readonly Stack<Color> s_ColorStack = new();

        public static PopDisposable<Matrix4x4> PushMatrix(Matrix4x4 matrix4X4) {
            s_MatrixStack.Push(matrix4X4);
            return new PopDisposable<Matrix4x4>(s_MatrixStack);
        }

        public static PopDisposable<Color> PushColor(Color color) {
            s_ColorStack.Push(color);
            return new PopDisposable<Color>(s_ColorStack);
        }

        public static void PopMatrix() => s_MatrixStack.Pop();
        public static void PopColor() => s_ColorStack.Pop();

        public static PopDisposable<Matrix4x4> ReplaceMatrix(Matrix4x4 matrix4X4) {
            s_MatrixStack.Pop();
            s_MatrixStack.Push(matrix4X4);
            return new PopDisposable<Matrix4x4>(s_MatrixStack);
        }

        public static PopDisposable<Color> ReplaceColor(Color color) {
            s_ColorStack.Pop();
            s_ColorStack.Push(color);
            return new PopDisposable<Color>(s_ColorStack);
        }

        public struct PopDisposable<T> : IDisposable {
            private readonly Stack<T> m_Stack;
            private bool m_Disposed;

            public PopDisposable(Stack<T> stack) {
                m_Stack = stack;
                m_Disposed = false;
            }

            public void Dispose() {
                if (m_Disposed) return;

                m_Stack.Pop();
                m_Disposed = true;
            }
        }
    }

#if UNITY_EDITOR
    public static partial class ExtraGizmos {
        /// <inheritdoc cref="Handles.DrawSolidDisc"/>
        public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius) {
            Color currentColor = Handles.color;
            Matrix4x4 currentMatrix = Handles.matrix;
            CompareFunction currentZTest = Handles.zTest;

            Handles.color = s_ColorStack.TryPeek(out Color color) ? color : currentColor;
            Handles.matrix = s_MatrixStack.TryPeek(out Matrix4x4 matrix) ? matrix : currentMatrix;
            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawSolidDisc(center, normal, radius);

            Handles.color = currentColor;
            Handles.matrix = currentMatrix;
            Handles.zTest = currentZTest;
        }

        /// <inheritdoc cref="Handles.DrawWireDisc(UnityEngine.Vector3,UnityEngine.Vector3,float)"/>
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius) {
            Color currentColor = Handles.color;
            Matrix4x4 currentMatrix = Handles.matrix;
            CompareFunction currentZTest = Handles.zTest;

            Handles.color = s_ColorStack.TryPeek(out Color color) ? color : currentColor;
            Handles.matrix = s_MatrixStack.TryPeek(out Matrix4x4 matrix) ? matrix : currentMatrix;
            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawWireDisc(center, normal, radius);

            Handles.color = currentColor;
            Handles.matrix = currentMatrix;
            Handles.zTest = currentZTest;
        }

        /// <inheritdoc cref="Handles.DrawWireDisc(UnityEngine.Vector3,UnityEngine.Vector3,float,float)"/>
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, float thickness) {
            Color currentColor = Handles.color;
            Matrix4x4 currentMatrix = Handles.matrix;
            CompareFunction currentZTest = Handles.zTest;

            Handles.color = s_ColorStack.TryPeek(out Color color) ? color : currentColor;
            Handles.matrix = s_MatrixStack.TryPeek(out Matrix4x4 matrix) ? matrix : currentMatrix;
            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawWireDisc(center, normal, radius, thickness);

            Handles.color = currentColor;
            Handles.matrix = currentMatrix;
            Handles.zTest = currentZTest;
        }

        /// <inheritdoc cref="Handles.DrawSolidArc"/>
        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, Degrees angle, float radius) {
            Color currentColor = Handles.color;
            Matrix4x4 currentMatrix = Handles.matrix;
            CompareFunction currentZTest = Handles.zTest;

            Handles.color = s_ColorStack.TryPeek(out Color color) ? color : currentColor;
            Handles.matrix = s_MatrixStack.TryPeek(out Matrix4x4 matrix) ? matrix : currentMatrix;
            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawSolidArc(center, normal, from, angle, radius);

            Handles.color = currentColor;
            Handles.matrix = currentMatrix;
            Handles.zTest = currentZTest;
        }

        /// <inheritdoc cref="Handles.DrawWireArc(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,float,float)"/>
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, Degrees angle, float radius) {
            Color currentColor = Handles.color;
            Matrix4x4 currentMatrix = Handles.matrix;
            CompareFunction currentZTest = Handles.zTest;

            Handles.color = s_ColorStack.TryPeek(out Color color) ? color : currentColor;
            Handles.matrix = s_MatrixStack.TryPeek(out Matrix4x4 matrix) ? matrix : currentMatrix;
            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawWireArc(center, normal, from, angle, radius);

            Handles.color = currentColor;
            Handles.matrix = currentMatrix;
            Handles.zTest = currentZTest;
        }

        /// <inheritdoc cref="Handles.DrawWireArc(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Vector3,float,float,float)"/>
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, Degrees angle, float radius, float thickness) {
            Color currentColor = Handles.color;
            Matrix4x4 currentMatrix = Handles.matrix;
            CompareFunction currentZTest = Handles.zTest;

            Handles.color = s_ColorStack.TryPeek(out Color color) ? color : currentColor;
            Handles.matrix = s_MatrixStack.TryPeek(out Matrix4x4 matrix) ? matrix : currentMatrix;
            Handles.zTest = CompareFunction.LessEqual;

            Handles.DrawWireArc(center, normal, from, angle, radius, thickness);

            Handles.color = currentColor;
            Handles.matrix = currentMatrix;
            Handles.zTest = currentZTest;
        }

        /// <inheritdoc cref="Handles.DrawSolidRectangleWithOutline(UnityEngine.Rect,UnityEngine.Color,UnityEngine.Color)"/>
        public static void DrawSolidRectangleWithOutline(Rect rect, Color faceColor, Color outlineColor) {
            Handles.DrawSolidRectangleWithOutline(rect, faceColor, outlineColor);
        }

        /// <inheritdoc cref="Handles.DrawSolidRectangleWithOutline(UnityEngine.Vector3[],UnityEngine.Color,UnityEngine.Color)"/>
        public static void DrawSolidRectangleWithOutline(Vector3[] points, Color faceColor, Color outlineColor) {
            Handles.DrawSolidRectangleWithOutline(points, faceColor, outlineColor);
        }
    }
#else
    public static partial class ExtraGizmos {
        public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius) {}
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius) {}
        public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, float thickness) {}
        public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, Degrees angle, float radius) {}
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, Degrees angle, float radius) {}
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, Degrees angle, float radius, float thickness) {}
        public static void DrawSolidRectangleWithOutline(Rect rect, Color faceColor, Color outlineColor) {}
        public static void DrawSolidRectangleWithOutline(Vector3[] points, Color faceColor, Color outlineColor) {}
    }
#endif
}
