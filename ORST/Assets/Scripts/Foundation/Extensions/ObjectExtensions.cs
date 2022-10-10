using UnityEngine;

namespace ORST.Foundation.Extensions {
    public static class ObjectExtensions {
        public static T OrNull<T>(this T obj) where T : class {
            return obj == null ? null : obj;
        }
    }
}