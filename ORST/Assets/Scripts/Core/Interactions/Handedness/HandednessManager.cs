using System;
using Oculus.Interaction.Input;
using ORST.Foundation.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.Interactions {
    public class HandednessManager : MonoSingleton<HandednessManager> {
        [SerializeField, Required] private Hand m_LeftHand;
        [SerializeField, Required] private Hand m_RightHand;

        public static event Action<Handedness> HandednessChanged = delegate { };

        /// <summary>
        /// Gets a value representing the dominant hand.
        /// </summary>
        public static Hand DominantHand => Instance.m_Handedness == Handedness.Left ? Instance.m_LeftHand : Instance.m_RightHand;

        /// <summary>
        /// Gets a value representing the non-dominant hand.
        /// </summary>
        public static Hand NonDominantHand => Instance.m_Handedness == Handedness.Left ? Instance.m_RightHand : Instance.m_LeftHand;

        /// <summary>
        /// Gets or sets the handedness.
        /// </summary>
        public static Handedness Handedness {
            get => Instance.m_Handedness;
            set {
                Instance.m_Handedness = value;
                HandednessChanged(value);
                PlayerPrefs.SetInt("ORST.Interactions.Handedness", (int)value);
            }
        }

        /// <summary>
        /// Gets a value representing the default <see cref="T:Oculus.Interaction.Input.Handedness"/>.
        /// </summary>
        /// <remarks>
        /// If handedness is not supported by the device (e.g. Unity Editor), then this property returns
        /// <see cref="Oculus.Interaction.Input.Handedness.Right"/>.
        /// </remarks>
        public static Handedness DefaultHandedness => OVRInput.GetDominantHand() switch {
            OVRInput.Handedness.Unsupported => Handedness.Right,

            // We need to convert between OVRInput.Handedness and Oculus.Interaction.Input.Handedness
            // because OVRInput.Handedness has an extra value (Unsupported) at the beginning.
            var handedness => (Handedness)((int)handedness - 1)
        };

        private Handedness m_Handedness;

        protected override void OnAwake() {
            m_Handedness = PlayerPrefs.HasKey("ORST.Interactions.Handedness")
                ? (Handedness)PlayerPrefs.GetInt("ORST.Interactions.Handedness")
                : DefaultHandedness;
        }
    }
}