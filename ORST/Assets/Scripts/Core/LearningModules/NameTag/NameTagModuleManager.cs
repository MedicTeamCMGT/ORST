using System;
using DG.Tweening;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.Attributes;
using ORST.Core.Effects;
using ORST.Core.UI.Components;
using ORST.Foundation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ORST.Core.LearningModules {
    public class NameTagModuleManager : BaseMonoBehaviour {
        public event Action NameTagConfirmed;

        [Title("References")]
        [SerializeField, Required, SdfIcon(SdfIconType.InboxFill)] private NameTagDropZone m_DropZone;
        [SerializeField, Required] private ParticleSystem m_ParticleSystem;
        [SerializeField, Required] private CanvasGroup m_CanvasGroup;
        [Space]
        [SerializeField, Required, SdfIcon(SdfIconType.TextareaT)] private TextMeshProUGUI m_HelpLabel;
        [SerializeField, Required, SdfIcon(SdfIconType.CheckSquareFill)] private VRButton m_ConfirmButton;
        [Space]
        [SerializeField, Required, SdfIcon(SdfIconType.TagsFill, Color = "#ff1c2c")] private NameTagObject m_EthiconNameTag;
        [SerializeField, Required, SdfIcon(SdfIconType.TagsFill, Color = "#1b99f7")] private NameTagObject m_DePuyNameTag;

        [Title("Settings")]
        [SerializeField, SuffixLabel("seconds")] private float m_ConfirmButtonAnimationDuration = 0.5f;

        [Title("Help Text")]
        [SerializeField, TextArea(3, 6)] private string m_DefaultMessage;
        [SerializeField, TextArea(3, 6)] private string m_SelectionMessage;
        [SerializeField, TextArea(3, 6)] private string m_ConfirmedMessage;

        [SerializeField] private OutlineGlow m_EthiconOutline;
        [SerializeField] private OutlineGlow m_DePuyOutline;

        private void Awake() {
            m_DropZone.NameTagChanged += OnNameTagChanged;
            m_ConfirmButton.Button.onClick.AddListener(OnConfirmButtonClicked);

            m_EthiconNameTag.PointableEventWrapper.WhenSelect.AddListener(OnNameTagPickedUp);
            m_EthiconNameTag.PointableEventWrapper.WhenUnselect.AddListener(OnNameTagReleased);
            m_DePuyNameTag.PointableEventWrapper.WhenSelect.AddListener(OnNameTagPickedUp);
            m_DePuyNameTag.PointableEventWrapper.WhenUnselect.AddListener(OnNameTagReleased);
        }

        private void OnConfirmButtonClicked() {
            NameTagConfirmed?.Invoke();

            m_HelpLabel.text = m_ConfirmedMessage.Replace("{Selection}", GetNameTagName(NameTag.Kind));
            HideConfirmButton();

            Destroy(m_EthiconNameTag.GetComponentInChildren<HandGrabInteractable>());
            Destroy(m_DePuyNameTag.GetComponentInChildren<HandGrabInteractable>());
            Destroy(m_EthiconNameTag.GetComponent<Grabbable>());
            Destroy(m_DePuyNameTag.GetComponent<Grabbable>());
        }

        private void Start() {
            UpdateHelpMessage(NameTagKind.None);
            HideConfirmButton();
            m_CanvasGroup.alpha = 0.0f;
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void HandleTaskStarted() {
            m_CanvasGroup.DOFade(1.0f, 0.5f);
            m_EthiconOutline.Show().OnComplete(() => m_EthiconOutline.StartGlow());
            m_DePuyOutline.Show().OnComplete(() => m_DePuyOutline.StartGlow());
        }

        private void OnNameTagChanged(NameTagKind nameTagKind) {
            NameTag.Kind = nameTagKind;

            UpdateHelpMessage(nameTagKind);

            if (nameTagKind == NameTagKind.None) {
                m_ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                HideConfirmButton();
            } else {
                m_ParticleSystem.Play();
                ShowConfirmButton();
            }

        }

        private void ShowConfirmButton() {
            m_ConfirmButton.CanvasGroup.blocksRaycasts = true;
            m_ConfirmButton.CanvasGroup.DOFade(1.0f, m_ConfirmButtonAnimationDuration);
        }

        private void HideConfirmButton() {
            m_ConfirmButton.CanvasGroup.blocksRaycasts = false;
            m_ConfirmButton.CanvasGroup.DOFade(0.0f, m_ConfirmButtonAnimationDuration);
        }

        private void OnNameTagPickedUp() {
            m_EthiconOutline.StopGlow();
        }

        private void OnNameTagReleased() {
            if (NameTag.Kind is NameTagKind.None) {
                m_EthiconOutline.StartGlow();
            }
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