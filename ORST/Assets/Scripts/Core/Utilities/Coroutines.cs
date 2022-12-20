using System;
using System.Collections;

namespace ORST.Core.Utilities {
    public static class Coroutines {
        public static IEnumerator WaitFramesAndThen(int frames, Action action) {
            for (int i = 0; i < frames; i++) {
                yield return null;
            }

            action?.Invoke();
        }
    }
}