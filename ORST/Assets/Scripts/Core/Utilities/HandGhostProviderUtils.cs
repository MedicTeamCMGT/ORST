using Oculus.Interaction.HandGrab.Visuals;
using UnityEngine;

namespace ORST.Core.Utilities {
    public static class HandGhostProviderUtils {
        public static bool TryGetDefault(out HandGhostProvider provider) {
            HandGhostProvider[] providers = Resources.FindObjectsOfTypeAll<HandGhostProvider>();
            if (providers != null && providers.Length > 0) {
                provider = providers[0];
                return true;
            }

            provider = null;
            return false;
        }
    }
}