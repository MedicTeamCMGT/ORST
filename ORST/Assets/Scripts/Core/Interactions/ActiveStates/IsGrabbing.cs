using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace ORST.Core.Interactions {
    /// <summary>
    /// An <see cref="IActiveState"/> implementation that is active when the
    /// provided <see cref="IHandGrabState"/> is grabbing an object.
    /// </summary>
    public class IsGrabbing : SerializedMonoBehaviour, IActiveState {
        [OdinSerialize, Required] private IHandGrabState m_HandGrabState;

        public bool Active => m_HandGrabState.IsGrabbing;
    }
}