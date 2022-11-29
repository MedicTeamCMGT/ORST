using System;
using DG.Tweening;
using ORST.Core.Interactions;
using ORST.Foundation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ORST.Core.UI {
    public class EnterNameUI : BaseMonoBehaviour {
        [SerializeField, SuffixLabel("seconds")] private float m_AnimationDuration = 0.5f;
        [SerializeField] private float m_TitleOffset = 92.0f;
        [Space]
        [SerializeField, Required] private TextMeshProUGUI m_TitleLabel;
        [SerializeField, Required] private TextMeshProUGUI m_InputLabel;
        [SerializeField, Required] private Button m_StartInputButton;
        [SerializeField, Required] private CanvasGroup m_StartInputCanvasGroup;
        [Space]
        [SerializeField, Required] private Button m_ConfirmButton;
        [SerializeField, Required] private CanvasGroup m_ConfirmButtonCanvasGroup;

        public event Action<string> NameConfirmed = delegate { };

        private string m_Name;
        private bool m_IsInputShown;

        private void Awake() {
            m_StartInputButton.onClick.RemoveAllListeners();
            m_StartInputButton.onClick.AddListener(StartInput);
            m_StartInputCanvasGroup.alpha = 1.0f;
            m_TitleLabel.alpha = 0.0f;

            m_ConfirmButtonCanvasGroup.alpha = 0.0f;
            m_ConfirmButtonCanvasGroup.blocksRaycasts = false;
            m_ConfirmButton.gameObject.SetActive(false);
            m_ConfirmButton.onClick.RemoveAllListeners();
            m_ConfirmButton.onClick.AddListener(ConfirmInput);
        }

        private void StartInput() {
            m_InputLabel.text = string.Empty;
            m_TitleLabel.rectTransform.DOKill();
            m_TitleLabel.rectTransform.DOAnchorPosY(0.0f, m_AnimationDuration);
            m_IsInputShown = false;

            HideStartInputButton();
            HideConfirmButton();
            ShowTitleLabel();

            KeyboardInput.Instance.StartInput(string.Empty, OnInputChanged, OnInputFinished);
        }

        private void OnInputChanged(string text) {
            m_InputLabel.text = text.Trim();

            bool isEmpty = string.IsNullOrWhiteSpace(m_InputLabel.text);
            if (isEmpty && m_IsInputShown) {
                m_TitleLabel.rectTransform.DOKill();
                m_TitleLabel.rectTransform.DOAnchorPosY(0.0f, m_AnimationDuration);
                (m_StartInputButton.transform as RectTransform)!.DOKill();
                (m_StartInputButton.transform as RectTransform)!.DOAnchorPosY(0.0f, m_AnimationDuration);
                m_IsInputShown = false;
            } else if (!isEmpty && !m_IsInputShown) {
                m_TitleLabel.rectTransform.DOKill();
                m_TitleLabel.rectTransform.DOAnchorPosY(m_TitleOffset, m_AnimationDuration);
                (m_StartInputButton.transform as RectTransform)!.DOKill();
                (m_StartInputButton.transform as RectTransform)!.DOAnchorPosY(m_TitleOffset, m_AnimationDuration);
                m_IsInputShown = true;
            }
        }

        private void OnInputFinished() {
            HideTitleLabel();
            ShowStartInputButton();

            m_Name = m_InputLabel.text.Trim();
            if (!string.IsNullOrWhiteSpace(m_Name)) {
                ShowConfirmButton();
            } else {
                HideConfirmButton();
            }
        }

        private void ConfirmInput() {
            NameConfirmed(m_InputLabel.text);
        }

        private void ShowConfirmButton() {
            m_ConfirmButton.gameObject.SetActive(true);
            m_ConfirmButtonCanvasGroup.blocksRaycasts = true;
            m_ConfirmButtonCanvasGroup.DOKill();
            m_ConfirmButtonCanvasGroup.DOFade(1.0f, m_AnimationDuration);
        }

        private void HideConfirmButton() {
            m_ConfirmButtonCanvasGroup.blocksRaycasts = false;
            m_ConfirmButtonCanvasGroup.DOKill();
            m_ConfirmButtonCanvasGroup.DOFade(0.0f, m_AnimationDuration).OnComplete(() => {
                m_ConfirmButton.gameObject.SetActive(false);
            });
        }

        private void ShowStartInputButton() {
            m_StartInputButton.gameObject.SetActive(true);
            m_StartInputCanvasGroup.blocksRaycasts = true;
            m_StartInputCanvasGroup.DOKill();
            m_StartInputCanvasGroup.DOFade(1.0f, m_AnimationDuration);
        }

        private void HideStartInputButton() {
            m_StartInputCanvasGroup.blocksRaycasts = false;
            m_StartInputCanvasGroup.DOKill();
            m_StartInputCanvasGroup.DOFade(0.0f, m_AnimationDuration).OnComplete(() => {
                m_StartInputButton.gameObject.SetActive(false);
            });
        }

        private void ShowTitleLabel() {
            m_TitleLabel.gameObject.SetActive(true);
            m_TitleLabel.DOKill();
            m_TitleLabel.DOFade(1.0f, m_AnimationDuration);
        }

        private void HideTitleLabel() {
            m_TitleLabel.DOKill();
            m_TitleLabel.DOFade(0.0f, m_AnimationDuration).OnComplete(() => {
                m_TitleLabel.gameObject.SetActive(false);
            });
        }
    }
}