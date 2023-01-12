using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace ORST.Core.Movement {
    public static class TeleportPointManager {
        private static readonly Dictionary<TeleportPointORST, TeleportPointInfo> s_TeleportPoints = new();
        private static HashSet<TeleportPointORST> s_RestrictedTeleportPoints;
        private static bool s_IsTeleportationEnabled = true;

        /// <summary>
        /// Event invoked when teleportation is enabled or disabled.
        /// </summary>
        public static event Action<bool> TeleportationEnabledChanged = delegate {  };

        /// <summary>
        /// Event invoked when teleportation is restricted or unrestricted.
        /// </summary>
        public static event Action<bool> TeleportationRestrictionChanged = delegate {  };

        /// <summary>
        /// Gets a value indicating whether teleportation is restricted.
        /// </summary>
        public static bool IsTeleportationRestricted => s_RestrictedTeleportPoints != null;

        /// <summary>
        /// Gets or sets a value indicating whether teleportation is enabled.
        /// </summary>
        public static bool IsTeleportationEnabled {
            get => s_IsTeleportationEnabled;
            set {
                if (s_IsTeleportationEnabled == value) {
                    return;
                }

                s_IsTeleportationEnabled = value;
                TeleportationEnabledChanged(value);
            }
        }

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
            if (!s_TeleportPoints.ContainsKey(teleportPoint)) {
                return;
            }

            s_TeleportPoints[teleportPoint].Enabled = true;
            teleportPoint.Visuals.SetActive(true);
        }

        /// <summary>
        /// Enables all <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void EnableAllPoints() {
            foreach (KeyValuePair<TeleportPointORST, TeleportPointInfo> teleportPoint in s_TeleportPoints) {
                teleportPoint.Value.Enabled = true;
                teleportPoint.Key.Visuals.SetActive(true);
            }
        }

        /// <summary>
        /// Disables teleportation to the given <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void DisablePoint(TeleportPointORST teleportPoint) {
            if (!s_TeleportPoints.ContainsKey(teleportPoint)) {
                return;
            }

            s_TeleportPoints[teleportPoint].Enabled = false;
            teleportPoint.Visuals.SetActive(false);
        }

        /// <summary>
        /// Disables all <see cref="TeleportPointORST"/>.
        /// </summary>
        public static void DisableAllPoints() {
            foreach(TeleportPointORST teleportPoint in s_TeleportPoints.Keys) {
                DisablePoint(teleportPoint);
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the given <see cref="TeleportPointORST"/> is available.
        /// </summary>
        public static bool IsAvailable(TeleportPointORST teleportPoint) {
            if (!IsTeleportationEnabled) {
                return false;
            }

            if (s_RestrictedTeleportPoints != null) {
                return s_RestrictedTeleportPoints.Contains(teleportPoint);
            }

            return s_TeleportPoints.TryGetValue(teleportPoint, out TeleportPointInfo info) && info.Enabled;
        }

        /// <summary>
        /// Restricts teleportation to the given <see cref="TeleportPointORST"/>s.
        /// </summary>
        public static void RestrictTeleportation(IEnumerable<TeleportPointORST> teleportPoints) {
            if (s_RestrictedTeleportPoints != null) {
                UnrestrictTeleportationImpl();
            }

            RestrictTeleportationImpl(teleportPoints);
            TeleportationRestrictionChanged(true);
        }

        /// <summary>
        /// Stops restricting teleportation.
        /// </summary>
        public static void UnrestrictTeleportation() {
            UnrestrictTeleportationImpl();
            TeleportationRestrictionChanged(false);
        }

        private static void RestrictTeleportationImpl(IEnumerable<TeleportPointORST> teleportPoints) {
            s_RestrictedTeleportPoints = HashSetPool<TeleportPointORST>.Get();
            s_RestrictedTeleportPoints.UnionWith(teleportPoints);
        }

        private static void UnrestrictTeleportationImpl() {
            HashSetPool<TeleportPointORST>.Release(s_RestrictedTeleportPoints);
            s_RestrictedTeleportPoints = null;
        }

        private class TeleportPointInfo {
            public bool Enabled { get; set; } = true;
        }
    }
}