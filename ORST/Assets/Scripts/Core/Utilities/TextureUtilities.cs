using UnityEngine;

namespace ORST.Core.Utilities {
    public static class TextureUtilities {
        /// <summary>
        /// Resizes a texture by blitting, this allows you to resize unreadable textures.
        /// </summary>
        public static Texture2D ResizeByBlit(this Texture texture, int width, int height, FilterMode filterMode = FilterMode.Bilinear) {
            RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            temporary.filterMode = FilterMode.Bilinear;


            RenderTexture oldActive = RenderTexture.active;
            RenderTexture.active = temporary;

            GL.Clear(false, true, new Color(1f, 1f, 1f, 0.0f));
            bool sRgbWrite = GL.sRGBWrite;
            GL.sRGBWrite = false;

            Graphics.Blit(texture, temporary);

            Texture2D texture2D = new(width, height, TextureFormat.ARGB32, true, false) {
                filterMode = filterMode
            };
            texture2D.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = oldActive;
            RenderTexture.ReleaseTemporary(temporary);

            GL.sRGBWrite = sRgbWrite;
            return texture2D;
        }
    }
}