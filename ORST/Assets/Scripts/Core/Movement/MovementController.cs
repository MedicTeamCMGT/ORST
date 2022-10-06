using Sirenix.OdinInspector;
using UnityEngine;

public class MovementController : MonoBehaviour {
    [SerializeField, Required] private OVRCameraRig m_OvrCameraRig;
    [SerializeField, Required] private CapsuleCollider m_CharacterCapsule;

    public OVRCameraRig OvrCameraRig => m_OvrCameraRig;
    public CapsuleCollider CapsuleCollider => m_CharacterCapsule;
}
