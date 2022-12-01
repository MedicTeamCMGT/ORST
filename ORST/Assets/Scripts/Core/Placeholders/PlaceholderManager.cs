using System.Collections.Generic;

namespace ORST.Core.Placeholders {
    public static class PlaceholderManager {
        private static readonly Dictionary<string, string> s_Placeholders = new();

        /// <summary>
        /// Tries to get the value of the given placeholder key.
        /// </summary>
        /// <param name="key">The key of the placeholder.</param>
        /// <param name="value">Out parameter for the value of the placeholder.</param>
        /// <returns><see langword="true"/> if the placeholder exists, otherwise <see langword="false"/>.</returns>
        public static bool TryGetPlaceholder(string key, out string value) {
            return s_Placeholders.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets the value of the given placeholder key, or the default value if the placeholder does not exist.
        /// </summary>
        /// <param name="key">The key of the placeholder.</param>
        /// <param name="defaultValue">The default value to return if the placeholder does not exist.</param>
        /// <returns>The value of the placeholder, or the default value if the placeholder does not exist.</returns>
        public static string GetValueOrDefault(string key, string defaultValue) {
            return s_Placeholders.GetValueOrDefault(key, defaultValue);
        }

        /// <summary>
        /// Registers a new placeholder with the given key and value.
        /// </summary>
        /// <param name="key">The key of the placeholder.</param>
        /// <param name="value">The value of the placeholder.</param>
        public static void AddPlaceholder(string key, string value) {
            s_Placeholders[key] = value;
        }

        /// <summary>
        /// Removes the placeholder with the given key.
        /// </summary>
        /// <param name="key">The key of the placeholder.</param>
        /// <returns><see langword="true"/> if the placeholder was removed, otherwise <see langword="false"/>.</returns>
        public static bool RemovePlaceholder(string key) {
            return s_Placeholders.Remove(key);
        }

        /// <summary>
        /// Resolves the given input string by replacing all placeholders with their values.
        /// </summary>
        /// <param name="input">The input string to resolve.</param>
        /// <returns>The resolved string.</returns>
        public static string Resolve(string input) {
            foreach ((string key, string value) in s_Placeholders) {
                input = input.Replace(key, value);
            }

            return input;
        }
    }
}