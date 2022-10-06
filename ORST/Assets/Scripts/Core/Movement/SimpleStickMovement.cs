using UnityEngine;
using UnityEngine.Assertions;

public class SimpleStickMovement : MonoBehaviour {
    [SerializeField] private bool m_EnableLinearMovement = true;
    [SerializeField] private bool m_EnableRotation = true;
    [SerializeField] private bool m_RotateEitherThumbstick;
    [SerializeField] private bool m_HMDRotatesPlayer;
    [SerializeField] private float m_RotationAngle = 45.0f;
    [SerializeField] private float m_Speed = 5.0f;

    [SerializeField] private OVRCameraRig m_OvrCameraRig;
    private Rigidbody m_Rigidbody;
    private bool m_ReadyToSnapTurn = true;
    
    private void Start() {
       m_Rigidbody = GetComponentInParent<Rigidbody>();
       Assert.IsNotNull(m_Rigidbody);
       Assert.IsNotNull(m_OvrCameraRig);
   }

    private void FixedUpdate() {
        if (m_HMDRotatesPlayer) {
            RotatePlayerToHMD();
        }

        if (m_EnableLinearMovement) {
            StickMovement();
        }

        if (m_EnableRotation) {
            SnapRotate();
        }
    }

    private void RotatePlayerToHMD() {
        Transform root = m_OvrCameraRig.trackingSpace;
        Transform centerEye = m_OvrCameraRig.centerEyeAnchor;

        Vector3 previousPos = root.position;
        Quaternion previousRot = root.rotation;

        transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

        //??????????????????
        root.position = previousPos;
        root.rotation = previousRot;
    }

    private void StickMovement() {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 moveDirection = Quaternion.Euler(new Vector3(0,  m_OvrCameraRig.centerEyeAnchor.rotation.eulerAngles.y, 0))
                              * new Vector3(primaryAxis.x, 0, primaryAxis.y);
        m_Rigidbody.MovePosition(m_Speed * Time.deltaTime * moveDirection + m_Rigidbody.position);
    }


    private void SnapRotate() {
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft)
         || (m_RotateEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft))) {
            if (!m_ReadyToSnapTurn) {
                return;
            }
            m_ReadyToSnapTurn = false;
            transform.RotateAround(m_OvrCameraRig.centerEyeAnchor.position, Vector3.up, -m_RotationAngle);
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)
              || (m_RotateEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight))) {
            if (!m_ReadyToSnapTurn) {
                return;
            }
            m_ReadyToSnapTurn = false;
            transform.RotateAround(m_OvrCameraRig.centerEyeAnchor.position, Vector3.up, m_RotationAngle);
        } else {
            m_ReadyToSnapTurn = true;
        }
    }
}
