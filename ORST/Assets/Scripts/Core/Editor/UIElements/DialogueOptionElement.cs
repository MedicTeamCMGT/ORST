using System.Linq;
using ORST.Core.Dialogues;
using ORST.Core.Editor.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ORST.Core.Editor.UIElements {
    public class DialogueOptionElement : VisualElement {
        public string Text {
            get => m_Text;
            set => m_TextField.value = value;
        }

        public string Feedback {
            get => m_Feedback;
            set => m_FeedbackField.value = value;
        }

        public bool IsCorrect {
            get => m_IsCorrect;
            set => m_IsCorrectToggle.value = value;
        }

        private string m_Text;
        private string m_Feedback;
        private bool m_IsCorrect;

        private readonly TextField m_TextField;
        private readonly TextField m_FeedbackField;
        private readonly Toggle m_IsCorrectToggle;
        private Dialogue m_Dialogue;
        private DialogueNode m_DialogueNode;
        private DialogueOption m_DialogueOption;

        private static string s_VisualTreeAssetPath;
        private static VisualTreeAsset s_VisualTreeAsset;

        public DialogueOptionElement() {
            if (string.IsNullOrEmpty(s_VisualTreeAssetPath)) {
                s_VisualTreeAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DialogueOptionElement t:VisualTreeAsset").FirstOrDefault());
            }

            AssetUtilities.LoadIfNull(ref s_VisualTreeAsset, s_VisualTreeAssetPath);

            if (s_VisualTreeAsset == null) {
                Debug.LogError("Could not load VisualTreeAsset for DialogueOptionElement");
                return;
            }

            s_VisualTreeAsset.CloneTree(this);

            m_TextField = this.Q<TextField>("dialogue-option__text-field");
            m_FeedbackField = this.Q<TextField>("dialogue-option__feedback-field");
            m_IsCorrectToggle = this.Q<Toggle>("dialogue-option__is-correct-toggle");
            this.Q<Button>("dialogue-option__remove-button").clicked += RemoveDialogueOption;

            m_TextField.RegisterValueChangedCallback(evt => {
                Debug.Log("Text changed");
                m_Text = evt.newValue;
                if (m_DialogueOption == null) {
                    return;
                }

                Undo.RegisterCompleteObjectUndo(m_Dialogue, "Change Dialogue Option Text");
                SerializedObject serializedObject = new(m_Dialogue);
                serializedObject.Update();
                m_DialogueOption.Text = evt.newValue;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            });

            m_FeedbackField.RegisterValueChangedCallback(evt => {
                m_Feedback = evt.newValue;
                if (m_DialogueOption == null) {
                    return;
                }

                SerializedObject serializedObject = new(m_Dialogue);
                serializedObject.Update();
                Undo.RegisterCompleteObjectUndo(m_Dialogue, "Change Dialogue Option Feedback");
                m_DialogueOption.Feedback = evt.newValue;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            });

            m_IsCorrectToggle.RegisterValueChangedCallback(evt => {
                m_IsCorrect = evt.newValue;
                m_FeedbackField.style.display = m_IsCorrect ? DisplayStyle.None : DisplayStyle.Flex;
                if (m_DialogueOption == null) {
                    return;
                }

                SerializedObject serializedObject = new(m_Dialogue);
                serializedObject.Update();
                Undo.RegisterCompleteObjectUndo(m_Dialogue, "Change Dialogue Option Correctness");
                m_DialogueOption.IsCorrect = evt.newValue;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            });

            m_FeedbackField.style.display = m_IsCorrect ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void Bind(Dialogue dialogue, DialogueNode dialogueNode, DialogueOption dialogueOption) {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            m_Dialogue = dialogue;
            m_DialogueNode = dialogueNode;
            m_DialogueOption = dialogueOption;

            UpdateFields();
        }

        private void OnUndoRedo() {
            if (m_DialogueOption == null) {
                return;
            }

            UpdateFields();
        }

        private void UpdateFields() {
            m_TextField.SetValueWithoutNotify(m_DialogueOption.Text);
            m_IsCorrectToggle.SetValueWithoutNotify(m_DialogueOption.IsCorrect);
            m_FeedbackField.SetValueWithoutNotify(m_DialogueOption.Feedback);

            m_FeedbackField.style.display = m_DialogueOption.IsCorrect ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void RemoveDialogueOption() {
            m_DialogueNode.Options.Remove(m_DialogueOption);
        }

        public new class UxmlFactory : UxmlFactory<DialogueOptionElement, UxmlTraits> {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private readonly UxmlStringAttributeDescription m_Text = new() { name = "text" };
            private readonly UxmlStringAttributeDescription m_Feedback = new() { name = "feedback" };
            private readonly UxmlBoolAttributeDescription m_IsCorrect = new() { name = "correct" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);

                ((DialogueOptionElement) ve).Text = m_Text.GetValueFromBag(bag, cc);
                ((DialogueOptionElement) ve).Feedback = m_Feedback.GetValueFromBag(bag, cc);
                ((DialogueOptionElement) ve).IsCorrect = m_IsCorrect.GetValueFromBag(bag, cc);
            }
        }
    }
}