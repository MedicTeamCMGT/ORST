using System;
using DG.Tweening;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.Attachments;
using ORST.Core.Attributes;
using ORST.Core.UI.Components;
using ORST.Core.Utilities;
using ORST.Foundation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ORST.Core.LearningModules {
    public class NameTagModuleManager : BaseMonoBehaviour {
        public event Action NameTagConfirmed;

        [Title("References")]
        [SerializeField, Required] private CanvasGroup m_CanvasGroup;
        [Space]
        [SerializeField, Required, SdfIcon(SdfIconType.TextareaT)] private TextMeshProUGUI m_HelpLabel;
        [SerializeField, Required, SdfIcon(SdfIconType.CheckSquareFill)] private VRButton m_ConfirmButton;
        [Space]
        [SerializeField, Required, SdfIcon(SdfIconType.TagsFill, Color = "#ff1c2c")] private NameTagObject m_EthiconNameTag;
        [SerializeField, Required, SdfIcon(SdfIconType.TagsFill, Color = "#1b99f7")] private NameTagObject m_DePuyNameTag;
        [Space]
        [SerializeField, Required] private AttachmentPoint m_NameTagAttachmentPoint;

        [Title("Settings")]
        [SerializeField, SuffixLabel("seconds")] private float m_ConfirmButtonAnimationDuration = 0.5f;

        [Title("Help Text")]
        [SerializeField, TextArea(3, 6)] private string m_DefaultMessage;
        [SerializeField, TextArea(3, 6)] private string m_SelectionMessage;
        [SerializeField, TextArea(3, 6)] private string m_ConfirmedMessage;

        private void Awake() {
            m_NameTagAttachmentPoint.ObjectAttached += OnNameTagAttached;
            m_NameTagAttachmentPoint.ObjectDetached += OnNameTagDetached;
            m_ConfirmButton.Button.onClick.AddListener(OnConfirmButtonClicked);

            m_EthiconNameTag.AttachableObject.Grabbed.AddListener(OnNameTagPickedUp);
            m_EthiconNameTag.AttachableObject.Released.AddListener(OnNameTagReleased);
            m_DePuyNameTag.AttachableObject.Grabbed.AddListener(OnNameTagPickedUp);
            m_DePuyNameTag.AttachableObject.Released.AddListener(OnNameTagReleased);

            UpdateHelpMessage(NameTagKind.None);
            HideConfirmButton();
            m_CanvasGroup.alpha = 0.0f;
            m_CanvasGroup.transform.parent.gameObject.SetActive(false);

            m_EthiconNameTag.gameObject.SetActive(false);
            m_DePuyNameTag.gameObject.SetActive(false);
            if (m_NameTagAttachmentPoint.VisualsTransform != null) {
                m_NameTagAttachmentPoint.VisualsTransform.gameObject.SetActive(false);
            }
        }

        public void HandleTaskStarted() {
            m_CanvasGroup.DOFade(1.0f, 0.5f);
            m_CanvasGroup.transform.parent.gameObject.SetActive(true);

            m_EthiconNameTag.OutlineGlow.Show().OnComplete(() => m_EthiconNameTag.OutlineGlow.StartGlow());
            m_DePuyNameTag.OutlineGlow.Show().OnComplete(() => m_DePuyNameTag.OutlineGlow.StartGlow());

            m_EthiconNameTag.gameObject.SetActive(true);
            m_DePuyNameTag.gameObject.SetActive(true);

            if (m_NameTagAttachmentPoint.VisualsTransform != null) {
                m_NameTagAttachmentPoint.VisualsTransform.gameObject.SetActive(true);
            }
        }

        private void OnConfirmButtonClicked() {
            NameTagConfirmed?.Invoke();

            m_HelpLabel.text = m_ConfirmedMessage.Replace("{Selection}", GetNameTagName(NameTag.Kind));
            HideConfirmButton();

            m_EthiconNameTag.OutlineGlow.Hide();
            m_DePuyNameTag.OutlineGlow.Hide();

            Destroy(m_EthiconNameTag.GetComponentInChildren<HandGrabInteractable>());
            Destroy(m_EthiconNameTag.GetComponent<Grabbable>());
            Destroy(m_EthiconNameTag.GetComponent<OneGrabFreeTransformer>());
            Destroy(m_EthiconNameTag.GetComponent<AttachableObject>());
            Destroy(m_EthiconNameTag.GetComponent<NameTagObject>());
            Destroy(m_DePuyNameTag.GetComponentInChildren<HandGrabInteractable>());
            Destroy(m_DePuyNameTag.GetComponent<OneGrabFreeTransformer>());
            Destroy(m_DePuyNameTag.GetComponent<Grabbable>());
            Destroy(m_DePuyNameTag.GetComponent<AttachableObject>());
            Destroy(m_DePuyNameTag.GetComponent<NameTagObject>());
            Destroy(m_NameTagAttachmentPoint);

            Destroy(NameTag.Kind == NameTagKind.Ethicon ? m_DePuyNameTag.gameObject : m_EthiconNameTag.gameObject);

            if (m_NameTagAttachmentPoint.VisualsTransform != null) {
                m_NameTagAttachmentPoint.VisualsTransform.gameObject.SetActive(false);
            }
        }

        private void OnNameTagAttached(AttachableObject attachableObject) {
            NameTagObject nameTagObject = GetAttachedNameTag(attachableObject);
            Debug.Log($"OnNameTagAttached: {attachableObject.name}");
            OnNameTagChanged(nameTagObject.Kind);
        }

        private void OnNameTagDetached(AttachableObject attachableObject) {
            Debug.Log($"OnNameTagDetached: {attachableObject.name}");
            OnNameTagChanged(NameTagKind.None);
        }

        private NameTagObject GetAttachedNameTag(AttachableObject attachableObject) {
            if (attachableObject == m_EthiconNameTag.AttachableObject) {
                return m_EthiconNameTag;
            }

            if (attachableObject == m_DePuyNameTag.AttachableObject) {
                return m_DePuyNameTag;
            }

            return null;
        }

        private void OnNameTagChanged(NameTagKind nameTagKind) {
            NameTag.Kind = nameTagKind;

            UpdateHelpMessage(nameTagKind);

            if (nameTagKind == NameTagKind.None) {
                HideConfirmButton();
            } else {
                ShowConfirmButton();
            }
        }

        private void ShowConfirmButton() {
            m_ConfirmButton.CanvasGroup.blocksRaycasts = true;
            m_ConfirmButton.CanvasGroup.DOKill();
            m_ConfirmButton.CanvasGroup.DOFade(1.0f, m_ConfirmButtonAnimationDuration);
        }

        private void HideConfirmButton() {
            m_ConfirmButton.CanvasGroup.blocksRaycasts = false;
            m_ConfirmButton.CanvasGroup.DOKill();
            m_ConfirmButton.CanvasGroup.DOFade(0.0f, m_ConfirmButtonAnimationDuration);
        }

        private void OnNameTagPickedUp(AttachableObject attachableObject) {
            NameTagObject nameTagObject = GetAttachedNameTag(attachableObject);
            nameTagObject.OutlineGlow.StopGlow();
        }

        private void OnNameTagReleased(AttachableObject attachableObject) {
            // Wait a bit before checking so we make sure all events fired
            // and NameTag.Kind is updated.
            StartCoroutine(Coroutines.WaitFramesAndThen(2, () => {
                if (NameTag.Kind is NameTagKind.None) {
                    m_EthiconNameTag.OutlineGlow.StartGlow();
                    m_DePuyNameTag.OutlineGlow.StartGlow();
                }
            }));
        }

        private void UpdateHelpMessage(NameTagKind nameTagKind) {
            string helpText = nameTagKind switch {
                NameTagKind.None => m_DefaultMessage,
                _ => m_SelectionMessage.Replace("{Selection}", GetNameTagName(nameTagKind))
            };

            // TODO: Maybe add some animation here so it's not so sudden
            m_HelpLabel.text = helpText;
        }

        private static string GetNameTagName(NameTagKind nameTagKind) {
            return nameTagKind switch {
                NameTagKind.Ethicon => "Ethicon",
                NameTagKind.DePuy => "DePuy Synthes",
                _ => "None"
            };
        }
    }
}