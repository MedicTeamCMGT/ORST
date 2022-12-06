using System;
using DG.Tweening;
using Oculus.Interaction;
using ORST.Core.UI.Components;
using ORST.Foundation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace ORST.Core.LearningModules {
    public class NameTagModuleManager : BaseMonoBehaviour {
        public event Action NameTagConfirmed;

        [Title("References")]
        [SerializeField, Required] private NameTagDropZone m_DropZone;
        [SerializeField, Required] private ParticleSystem m_ParticleSystem;
        [Space]
        [SerializeField, Required] private TextMeshProUGUI m_HelpLabel;
        [SerializeField, Required] private VRButton m_ConfirmButton;
        [Space]
        [SerializeField, Required] private Grabbable m_EthiconNameTag;
        [SerializeField, Required] private Grabbable m_DePuyNameTag;

        [Title("Help Text")]
        [SerializeField, TextArea(3, 6)] private string m_DefaultMessage;
        [SerializeField, TextArea(3, 6)] private string m_SelectionMessage;
        [SerializeField, TextArea(3, 6)] private string m_ConfirmedMessage;

        [Title("Settings")]
        [SerializeField, SuffixLabel("seconds")] private float m_ConfirmButtonAnimationDuration = 0.5f;


        private void Awake() {
            m_DropZone.NameTagChanged += OnNameTagChanged;
            m_ConfirmButton.Button.onClick.AddListener(OnConfirmButtonClicked);
        }

        private void OnConfirmButtonClicked() {
            NameTagConfirmed?.Invoke();

            m_HelpLabel.text = m_ConfirmedMessage.Replace("{Selection}", GetNameTagName(NameTag.Kind));

            // Note: Setting MaxGrabPoints to 0 will essentially prevent the user
            // from grabbing the name tag again.
            m_EthiconNameTag.MaxGrabPoints = 0;
            m_DePuyNameTag.MaxGrabPoints = 0;
        }

        private void Start() {
            UpdateHelpMessage(NameTagKind.None);
            HideConfirmButton();
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