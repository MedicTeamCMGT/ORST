using System.Collections.Generic;
using ORST.Foundation.Random;

namespace ORST.Foundation.Extensions {
    public static class ListExtensions {
        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Rand.Range(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}