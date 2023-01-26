using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ORST.Core.Dialogues;
using ORST.Foundation;
using Sirenix.OdinInspector;
using ORST.Core.Placeholders;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ORST.Core.UI;

namespace ORST.Core {
    public class TutorialDialogueView : BaseMonoBehaviour {
        [SerializeField, Required] private TMP_Text m_Title;
        [SerializeField, Required] private TMP_Text m_Text;
        [SerializeField, Required] private List<Image> m_Images;
        [SerializeField, Required] private Button m_Button;

        private Action m_OptionSelectedCallback;
        private TutorialDialogueNode m_CurrentNode;

        private void Start() {
            m_Button.onClick.AddListener(() => m_OptionSelectedCallback?.Invoke());
        }

        public void Initialize([NotNull] Action optionSelectedCallback) {
            m_OptionSelectedCallback = optionSelectedCallback ?? throw new ArgumentNullException(nameof(optionSelectedCallback));
        }

        public void EnableButton() {
            m_Button.gameObject.SetActive(true);
        }

        public void DisableButton() {
            m_Button.gameObject.SetActive(false);
        }

        public void SetButtonText(string text) {
            m_Button.GetComponentInChildren<TMP_Text>().text = text;
        }

        public void LoadState(TutorialDialogueNode node) {
            m_CurrentNode = node;
            m_Title.text = PlaceholderManager.Resolve(node.Title);
            m_Text.text = PlaceholderManager.Resolve(node.Text);
            //PopupManager.Instance.ClosePopup();

            int index = 0;
            foreach (Sprite dialogueSprite in node.Images) {
                m_Images[index].sprite = dialogueSprite;
                m_Images[index].gameObject.SetActive(true);
                index++;
            }
            
            for (int i = index; i < m_Images.Count; i++) {
                m_Images[i].gameObject.SetActive(false);
            }

            // if (node.ShowPopup) {
            //     PopupManager.Instance.DisplayInfo(node.PopupTitle, node.PopupText);
            // }
        }
    }
}
