﻿using System;
using System.Collections;
using DG.Tweening;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using ORST.Core.Attachments;
using ORST.Core.Attributes;
using ORST.Core.UI.Components;
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
        }

        private void OnConfirmButtonClicked() {
            NameTagConfirmed?.Invoke();

            m_HelpLabel.text = m_ConfirmedMessage.Replace("{Selection}", GetNameTagName(NameTag.Kind));
            HideConfirmButton();

            Destroy(m_EthiconNameTag.GetComponentInChildren<HandGrabInteractable>());
            Destroy(m_EthiconNameTag.GetComponent<Grabbable>());
            Destroy(m_EthiconNameTag.GetComponent<AttachableObject>());
            Destroy(m_EthiconNameTag.GetComponent<NameTagObject>());
            Destroy(m_DePuyNameTag.GetComponentInChildren<HandGrabInteractable>());
            Destroy(m_DePuyNameTag.GetComponent<Grabbable>());
            Destroy(m_DePuyNameTag.GetComponent<AttachableObject>());
            Destroy(m_DePuyNameTag.GetComponent<NameTagObject>());
            Destroy(m_NameTagAttachmentPoint);
        }

        private void Start() {
            UpdateHelpMessage(NameTagKind.None);
            HideConfirmButton();
            m_CanvasGroup.alpha = 0.0f;
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void HandleTaskStarted() {
            m_CanvasGroup.DOFade(1.0f, 0.5f);
            m_EthiconNameTag.OutlineGlow.Show().OnComplete(() => m_EthiconNameTag.OutlineGlow.StartGlow());
            m_DePuyNameTag.OutlineGlow.Show().OnComplete(() => m_DePuyNameTag.OutlineGlow.StartGlow());
        }

        private void OnNameTagAttached(AttachableObject attachableObject) {
            NameTagObject nameTagObject = GetAttachedNameTag(attachableObject);
            OnNameTagChanged(nameTagObject.Kind);
        }

        private void OnNameTagDetached(AttachableObject attachableObject) {
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
            StartCoroutine(WaitFramesAndThen(2, () => {
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

        private IEnumerator WaitFramesAndThen(int frames, Action action) {
            for (int i = 0; i < frames; i++) {
                yield return null;
            }

            action?.Invoke();
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