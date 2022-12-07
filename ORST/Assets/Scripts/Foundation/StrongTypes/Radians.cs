using System;
using UnityEngine;

namespace ORST.Foundation.StrongTypes {
    /// <summary>
    /// Represents a value in radians.
    /// </summary>
    [Serializable]
    public struct Radians : IComparable<Radians>, IEquatable<Radians> {
        [SerializeField] private float m_Value;

        public float Value => m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Radians"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Radians(float value) {
            m_Value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="float"/> to <see cref="Radians"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Radians(float value) {
            return new Radians(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Radians"/> to <see cref="float"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float(Radians value) {
            return value.m_Value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Radians"/> to <see cref="Degrees"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Radians(Degrees value) {
            return new Radians(value.Value * Mathf.Deg2Rad);
        }

        public int CompareTo(Radians other) {
            return m_Value.CompareTo(other.m_Value);
        }

        public bool Equals(Radians other) {
            return m_Value.Equals(other.m_Value);
        }
    }
}