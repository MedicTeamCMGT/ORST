using UnityEngine;

namespace ORST.Core {
    public class SetParentHelper : MonoBehaviour {
        public void SetParentToNull() {
            transform.SetParent(null);
        }
    }
}