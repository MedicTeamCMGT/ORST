using System;
using UnityEngine;

namespace ORST.Foundation.StrongTypes {
    /// <summary>
    /// Represents a value in degrees.
    /// </summary>
    [Serializable]
    public struct Degrees : IComparable<Degrees>, IEquatable<Degrees> {
        [SerializeField] private float m_Value;

        public float Value => m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Degrees"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Degrees(float value) {
            m_Value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="float"/> to <see cref="Degrees"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Degrees(float value) {
            return new Degrees(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Degrees"/> to <see cref="float"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float(Degrees value) {
            return value.m_Value;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Radians"/> to <see cref="Degrees"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Degrees(Radians value) {
            return new Degrees(value.Value * Mathf.Rad2Deg);
        }

        public int CompareTo(Degrees other) {
            return m_Value.CompareTo(other.m_Value);
        }

        public bool Equals(Degrees other) {
            return m_Value.Equals(other.m_Value);
        }
    }
}