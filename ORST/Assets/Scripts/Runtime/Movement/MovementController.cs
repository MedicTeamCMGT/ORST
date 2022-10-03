using UnityEngine;

public class MovementController : MonoBehaviour {
    [SerializeField] private OVRCameraRig m_OvrCameraRig;
    [SerializeField] private CapsuleCollider m_CharacterCapsule;
    [SerializeField] private SimpleStickMovement m_SimpleStickMovement;

    public OVRCameraRig OvrCameraRig => m_OvrCameraRig;
    public CapsuleCollider CapsuleCollider => m_CharacterCapsule;
}
