using System;
using JetBrains.Annotations;
using UnityEngine;

namespace ORST.Core.Utilities {
    public static class MonoBehaviourStartExtensions {
        internal static StartScopeDisposable StartScope(this MonoBehaviour monoBehaviour, [NotNull] Func<bool> getStarted, [NotNull] Action<bool> setStarted, Action baseStart = null) {
            if (getStarted == null) throw new ArgumentNullException(nameof(getStarted));
            if (setStarted == null) throw new ArgumentNullException(nameof(setStarted));

            if (!getStarted()) {
                monoBehaviour.enabled = false;
                setStarted(true);
                baseStart?.Invoke();
                setStarted(false);
            } else {
                baseStart?.Invoke();
            }

            return new StartScopeDisposable(monoBehaviour, getStarted, setStarted);
        }
    }

    internal struct StartScopeDisposable : IDisposable {
        private readonly MonoBehaviour m_MonoBehaviour;
        private readonly Func<bool> m_GetStarted;
        private readonly Action<bool> m_SetStarted;
        private bool m_Disposed;

        public StartScopeDisposable(MonoBehaviour monoBehaviour, Func<bool> getStarted, Action<bool> setStarted) {
            m_MonoBehaviour = monoBehaviour;
            m_GetStarted = getStarted;
            m_SetStarted = setStarted;
            m_Disposed = false;
        }

        public void Dispose() {
            if (m_Disposed) {
                return;
            }

            m_Disposed = true;
            if (m_GetStarted()) {
                return;
            }

            m_SetStarted(true);
            m_MonoBehaviour.enabled = true;
        }
    }

}