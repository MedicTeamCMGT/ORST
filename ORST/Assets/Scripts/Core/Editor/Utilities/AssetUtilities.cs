using UnityEditor;
using UnityEngine;

namespace ORST.Core.Editor.Utilities {
    public static class AssetUtilities {
        /// <summary>
        /// Loads the <typeparamref name="T"/> asset using <see cref="AssetDatabase"/> if the given instance is null.
        /// </summary>
        /// <param name="obj">Object to assign</param>
        /// <param name="assetPath">Asset path</param>
        public static void LoadIfNull<T>(ref T obj, string assetPath) where T : Object {
            if (obj != null) {
                return;
            }

            obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}