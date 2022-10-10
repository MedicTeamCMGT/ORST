using UnityEngine;

namespace ORST.Foundation.Singleton {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
        private static T s_Instance;

        public static T Instance {
            get {
                if (s_Instance == null) {
                    s_Instance = FindObjectOfType<T>();
                }

                if (s_Instance == null) {
                    Debug.LogError($"No instance of {typeof(T).Name} found");
                }

                return s_Instance;
            }
        }

        private void Awake() {
            if (s_Instance != null && s_Instance != this) {
                Debug.LogError($"Multiple instances of {typeof(T).Name} found");
                OnAwake();
                Destroy(this);
                return;
            }

            s_Instance = this as T;
            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}