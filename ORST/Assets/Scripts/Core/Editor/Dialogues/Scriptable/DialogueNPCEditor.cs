using System.Collections.Generic;
using ORST.Core.Dialogues;
using ORST.Core.Utilities;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ORST.Core.Editor.Dialogues {
    [CustomEditor(typeof(DialogueNPC))]
    public class DialogueNPCEditor : OdinEditor {
        private static readonly Dictionary<int, CachedPreview> s_CachedPreviews = new();

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            DialogueNPC npc = target as DialogueNPC;
            Texture2D previewTexture = null;

            if (s_CachedPreviews.ContainsKey(npc!.GetInstanceID())) {
                CachedPreview cachedPreview = s_CachedPreviews[npc.GetInstanceID()];
                if (cachedPreview.Icon == npc.Icon) {
                    previewTexture = cachedPreview.PreviewTexture;
                } else {
                    s_CachedPreviews.Remove(npc.GetInstanceID());
                }
            }

            if (previewTexture != null) {
                return previewTexture;
            }

            if (npc!.Icon == null) {
                return base.RenderStaticPreview(assetPath, subAssets, width, height);
            }

            Texture2D texture;
            s_CachedPreviews[npc.GetInstanceID()] = new CachedPreview {
                Icon = npc.Icon,
                PreviewTexture = texture = npc.Icon.texture.ResizeByBlit(width, height)
            };

            return texture;
        }

        private struct CachedPreview {
            public Sprite Icon;
            public Texture2D PreviewTexture;
        }
    }
}