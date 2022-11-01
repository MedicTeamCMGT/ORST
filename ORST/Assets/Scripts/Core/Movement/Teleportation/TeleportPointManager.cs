using System.Collections.Generic;
using UnityEngine.Pool;

namespace ORST.Core.Movement {
    public static class TeleportPointManager {
        private static readonly Dictionary<TeleportPointORST, TeleportPointInfo> s_TeleportPoints = new();
        private static HashSet<TeleportPointORST> s_RestrictedTeleportPoints;

        /// <summary>
        /// Gets a value indicating whether teleportation is restricted.
        /// </summary>
        public static bool IsTeleportationRestricted => s_RestrictedTeleportPoints != null;

        /// <summary>
        /// Registers the given <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void RegisterPoint(TeleportPointORST teleportPoint) {
            s_TeleportPoints[teleportPoint] = new TeleportPointInfo();
        }

        /// <summary>
        /// Unregisters the given <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void UnregisterPoint(TeleportPointORST teleportPoint) {
            s_TeleportPoints.Remove(teleportPoint);
        }

        /// <summary>
        /// Enables teleportation to the given <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void EnablePoint(TeleportPointORST teleportPoint) {
            s_TeleportPoints[teleportPoint].Enabled = true;
        }

        /// <summary>
        /// Disables teleportation to the given <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void DisablePoint(TeleportPointORST teleportPoint) {
            s_TeleportPoints[teleportPoint].Enabled = false;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the given <see cref="TeleportPointORST"/> is available.
        /// </summary>
        public static bool IsAvailable(TeleportPointORST teleportPoint) {
            if (s_RestrictedTeleportPoints != null) {
                return s_RestrictedTeleportPoints.Contains(teleportPoint);
            }

            return s_TeleportPoints.TryGetValue(teleportPoint, out TeleportPointInfo info) && info.Enabled;
        }

        /// <summary>
        /// Restricts teleportation to the given <see cref="TeleportPointORST"/>s.
        /// </summary>
        public static void RestrictTeleportation(HashSet<TeleportPointORST> teleportPoints) {
            if (s_RestrictedTeleportPoints != null) {
                UnrestrictTeleportation();
            }

            s_RestrictedTeleportPoints = teleportPoints;
            foreach (TeleportPointORST teleportPoint in teleportPoints) {
                s_TeleportPoints[teleportPoint].Enabled = true;
            }
        }

        /// <summary>
        /// Unrestricts teleportation.
        /// </summary>
        public static void UnrestrictTeleportation() {
            HashSetPool<TeleportPointORST>.Release(s_RestrictedTeleportPoints);
            s_RestrictedTeleportPoints = null;
        }

        private class TeleportPointInfo {
            public bool Enabled { get; set; } = true;
        }
    }
}