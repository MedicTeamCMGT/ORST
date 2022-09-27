using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oculus.Interaction;
using Oculus.Interaction.Grab;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.HandGrab.Visuals;
using Oculus.Interaction.Input;
using ORST.Runtime.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Taken, adapted and fixed from: https://github.com/dilmerv/MetaInteractionSDKDemos/blob/978ebb2fa45e76c87dbe018ebcb0d8385733fedf/Assets/Scripts/HandPoseRecorderPlus.cs
namespace ORST.Runtime.Interactions {
    public class HandPoseRecorderPlus : MonoBehaviour {
        [Title("Reference interactor used for Hand Grab:")]
        [SerializeField, Required] private HandGrabInteractor m_HandGrabInteractor;

        [Title("Root of the object to record the pose to:")]
        [SerializeField, Required] private GameObject m_Recordable;

        [Tooltip("Prototypes of the static hands (ghosts) that visualize holding poses")]
        [SerializeField, Sirenix.OdinInspector.Optional] private HandGhostProvider m_GhostProvider;

        [SerializeField] private GrabTypeFlags m_GrabTypeFlags = GrabTypeFlags.All;

        [Tooltip("Collection for storing generated HandGrabInteractables during Play-Mode, so they can be restored in Edit-Mode")]
        [SerializeField, Sirenix.OdinInspector.Optional] private HandGrabInteractableDataCollection m_PosesCollection;

        [Title("Recording Settings")]
        [SerializeField] private KeyCode m_RecordStartKey = KeyCode.R;
        [SerializeField] private KeyCode m_RecordStopKey = KeyCode.Space;
        [SerializeField] private int m_RecordFrequency = 2;
        [SerializeField, Required] private TextMeshProUGUI m_RecordTimerText;

        private float m_RecordFrequencyTimer;
        private bool m_RecordFrequencyStarted;

        [Title("2- Store your poses before exiting Play mode: ")]
        [InspectorButton(nameof(SaveToAsset))]
        [SerializeField] private string m_StorePoses;

        [Title("3-Now load the poses in Edit mode to tweak and persist them: ")]
        [InspectorButton(nameof(LoadFromAsset))]
        [SerializeField] private string m_LoadPoses;

        private void Awake() {
            m_RecordTimerText.text = m_RecordFrequency.ToString("0");

            if (m_GhostProvider == null) {
                HandGhostProviderUtils.TryGetDefault(out m_GhostProvider);
            }
        }

        private void Update() {
            if (Input.GetKeyDown(m_RecordStopKey)) {
                Debug.Log($"Recording stopped");
                m_RecordFrequencyStarted = false;
            }

            if (Input.GetKeyDown(m_RecordStartKey)) {
                Debug.Log($"Recording started");
                m_RecordFrequencyStarted = true;
                m_RecordFrequencyTimer = m_RecordFrequency;
            }

            if (m_RecordFrequencyStarted) {
                if (m_RecordFrequencyTimer > 0) {
                    m_RecordFrequencyTimer -= Time.deltaTime;
                    m_RecordTimerText.text = $"{Mathf.CeilToInt(m_RecordFrequencyTimer)}";
                } else {
                    Debug.Log($"Recording hand pose for game object {m_Recordable.name}");
                    RecordPose();
                    m_RecordFrequencyTimer = m_RecordFrequency;
                }
            }
        }

        private void RecordPose() {
            Debug.Log($"Recording hand pose for game object {m_Recordable.name}");

            if (m_HandGrabInteractor == null || m_HandGrabInteractor.Hand == null) {
                Debug.LogError("Missing HandGrabInteractor. Ensure you are in PLAY mode!");
                return;
            }

            if (m_Recordable == null) {
                Debug.LogError("Missing Recordable");
                return;
            }

            HandPose trackedHandPose = TrackedPose();
            if (trackedHandPose == null) {
                Debug.LogError("Tracked Pose could not be retrieved");
                return;
            }

            Pose gripPoint = m_Recordable.transform.Delta(m_HandGrabInteractor.transform);
            HandGrabPose point = AddHandGrabPoint(trackedHandPose, gripPoint);
            AttachGhost(point);
        }

        private HandPose TrackedPose() {
            if (!m_HandGrabInteractor.Hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints)) {
                return null;
            }

            HandPose result = new(m_HandGrabInteractor.Hand.Handedness);
            for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; ++i) {
                HandJointId jointID = FingersMetadata.HAND_JOINT_IDS[i];
                result.JointRotations[i] = localJoints[jointID].rotation;
            }

            return result;
        }

        private void AttachGhost(HandGrabPose point) {
            if (m_GhostProvider == null) {
                return;
            }

            HandGhost ghostPrefab = m_GhostProvider.GetHand(m_HandGrabInteractor.Hand.Handedness);
            HandGhost ghost = Instantiate(ghostPrefab, point.transform);
            ghost.SetPose(point);
        }

        public HandGrabPose AddHandGrabPoint(HandPose rawPose, Pose snapPoint) {
            HandGrabInteractable interactable = HandGrabInteractable.Create(m_Recordable.transform);
            interactable.InjectSupportedGrabTypes(m_GrabTypeFlags);
            interactable.InjectRigidbody(m_Recordable.GetComponent<Rigidbody>());

            HandGrabPoseData pointData = new() {
                handPose = rawPose,
                scale = 1.0f,
                gripPose = snapPoint,
            };
            return interactable.LoadHandGrabPose(pointData);
        }

        private HandGrabInteractable LoadHandGrabInteractable(HandGrabInteractableData data) {
            HandGrabInteractable interactable = HandGrabInteractable.Create(m_Recordable.transform);
            interactable.InjectSupportedGrabTypes(m_GrabTypeFlags);
            interactable.InjectRigidbody(m_Recordable.GetComponent<Rigidbody>());

            interactable.LoadData(data);
            return interactable;
        }

        private void LoadFromAsset() {
            if (m_PosesCollection == null) {
                return;
            }

            foreach (HandGrabInteractableData handPose in m_PosesCollection.InteractablesData) {
                LoadHandGrabInteractable(handPose);
            }
        }

        private void SaveToAsset() {
            List<HandGrabInteractableData> savedPoses = m_Recordable.GetComponentsInChildren<HandGrabInteractable>(false)
                                                                    .Select(snap => snap.SaveData())
                                                                    .ToList();

            if (m_PosesCollection == null) {
                GenerateCollectionAsset();
            }

            m_PosesCollection.StoreInteractables(savedPoses);
        }

        private void GenerateCollectionAsset() {
#if UNITY_EDITOR
            m_PosesCollection = ScriptableObject.CreateInstance<HandGrabInteractableDataCollection>();
            string parentDir = Path.Combine("Assets", "HandGrabInteractableDataCollection");
            if (!Directory.Exists(parentDir)) {
                Directory.CreateDirectory(parentDir);
            }

            AssetDatabase.CreateAsset(m_PosesCollection, Path.Combine(parentDir, $"{(m_Recordable != null ? m_Recordable.name : "Auto")}_HandGrabCollection.asset"));
            AssetDatabase.SaveAssets();
#endif
        }
    }
}