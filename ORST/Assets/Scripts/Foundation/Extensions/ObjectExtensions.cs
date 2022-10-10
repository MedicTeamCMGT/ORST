using UnityEngine;

namespace ORST.Foundation.Extensions {
    public static class ObjectExtensions {
        public static T OrNull<T>(this T obj) where T : class {
            // ReSharper disable once MergeConditionalExpression, ConditionIsAlwaysTrueOrFalse
            // Convert Unity fake-null to real null
            if (obj is Object) return obj == null ? null : obj;

            return obj;
        }
    }
}