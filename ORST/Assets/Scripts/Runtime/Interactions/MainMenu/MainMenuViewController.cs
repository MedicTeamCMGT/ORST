using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ORST.Runtime.Interactions {
    public class MainMenuViewController : SerializedMonoBehaviour {
        [Title("Main Menu")]
        [SerializeField, Required] private RectMask2D m_SidebarRectMask;
        [SerializeField, Required] private RectMask2D m_MainRectMask;
        [SerializeField, Required] private CanvasGroup m_CanvasGroup;
        [Space]
        [SerializeField] private Vector2 m_RectMaskRightValues = new(700.0f, -16.0f);
        [SerializeField, SuffixLabel("seconds")] private float m_ShowMenuDuration = 0.25f;
        [SerializeField, SuffixLabel("seconds")] private float m_ShowPageDuration = 0.25f;

        [Title("Buttons/Pages")]
        [SerializeField, Required] private PokeInteractableToggleGroup m_ToggleGroup;
        [OdinSerialize, Required] private Dictionary<PokeInteractableToggle, MainMenuPage> m_TogglePages = new();

        private PokeInteractableToggle m_SelectedToggle;
        private MainMenuPage m_SelectedPage;
        private bool m_IsMenuVisible;
        private Sequence m_ShowMenuSequence;
        private Sequence m_HideMenuSequence;

        private void Awake() {
            Assert.IsNotNull(m_ToggleGroup);

            m_ToggleGroup.ToggleSelected += OnToggleSelected;
        }

        private void Start() {
            foreach (MainMenuPage page in m_TogglePages.Values) {
                page.CanvasGroup.alpha = 0.0f;

                if (!page.gameObject.activeSelf) {
                    page.gameObject.SetActive(true);
                }
            }

            Vector4 sidebarMaskPadding = m_SidebarRectMask.padding;
            sidebarMaskPadding.z = m_RectMaskRightValues.x;
            m_SidebarRectMask.padding = sidebarMaskPadding;
            Vector4 mainMaskPadding = m_MainRectMask.padding;
            mainMaskPadding.z = m_RectMaskRightValues.x;
            m_MainRectMask.padding = mainMaskPadding;

            m_CanvasGroup.alpha = 1.0f;
        }

        private void OnToggleSelected(PokeInteractableToggle toggle) {
            if (toggle == m_SelectedToggle) {
                return;
            }

            m_SelectedToggle = toggle;
            if (toggle == null) {
                HideMenu();
            } else {
                ShowMenu();
            }
        }

        private void ShowMenu() {
            Assert.IsNotNull(m_SelectedToggle);

            if (!m_IsMenuVisible) {
                m_IsMenuVisible = true;
                if (m_HideMenuSequence is { active: true }) {
                    m_HideMenuSequence.Kill();
                    m_HideMenuSequence = null;
                }

                if (m_ShowMenuSequence is { active: true }) {
                    m_ShowMenuSequence.Kill();
                    m_ShowMenuSequence = null;
                }

                m_ShowMenuSequence = DOTween.Sequence().Join(DOVirtual.Float(m_RectMaskRightValues.x, m_RectMaskRightValues.y, m_ShowMenuDuration, value => {
                    Vector4 padding = m_SidebarRectMask.padding;
                    padding.z = value;
                    m_SidebarRectMask.padding = padding;

                    Vector4 mainPadding = m_MainRectMask.padding;
                    mainPadding.z = value;
                    m_MainRectMask.padding = mainPadding;
                }));
                m_ShowMenuSequence.Play();
            }

            if (m_TogglePages.GetValueOrDefault(m_SelectedToggle) is { } page) {
                ShowPage(page);
                return;
            }

            Debug.LogError($"No page found for toggle {m_SelectedToggle.name}");
        }

        private void ShowPage(MainMenuPage page) {
            if (m_SelectedPage != null) {
                m_SelectedPage.Hide(m_ShowPageDuration);
            }

            m_SelectedPage = page;

            if (m_SelectedPage != null) {
                m_SelectedPage.Show(m_ShowPageDuration);
            }
        }

        private void HideMenu() {
            if (!m_IsMenuVisible) {
                return;
            }

            m_IsMenuVisible = false;

            if (m_HideMenuSequence is { active: true }) {
                m_HideMenuSequence.Kill();
                m_HideMenuSequence = null;
            }

            if (m_ShowMenuSequence is { active: true }) {
                m_ShowMenuSequence.Kill();
                m_ShowMenuSequence = null;
            }

            m_HideMenuSequence = DOTween.Sequence().Join(DOVirtual.Float(m_RectMaskRightValues.y, m_RectMaskRightValues.x, m_ShowMenuDuration, value => {
                Vector4 padding = m_SidebarRectMask.padding;
                padding.z = value;
                m_SidebarRectMask.padding = padding;

                Vector4 mainPadding = m_MainRectMask.padding;
                mainPadding.z = value;
                m_MainRectMask.padding = mainPadding;
            }));
            m_HideMenuSequence.Play();
        }
    }
}