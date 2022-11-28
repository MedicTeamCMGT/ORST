using System.Linq;
using ORST.Core.Dialogues;
using ORST.Core.Editor.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ORST.Core.Editor.UIElements {
    public class DialogueNodeElement  : VisualElement {
        private readonly TextField m_TextField;
        private readonly ScrollView m_OptionsScrollView;
        private readonly IMGUIContainer m_UpdateChecker;
        private Dialogue m_Dialogue;
        private DialogueNode m_DialogueNode;
        private int m_LastCollectionCount = -1;

        private static string s_VisualTreeAssetPath;
        private static VisualTreeAsset s_VisualTreeAsset;

        public DialogueNodeElement() {
            if (string.IsNullOrEmpty(s_VisualTreeAssetPath)) {
                s_VisualTreeAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DialogueNodeElement t:VisualTreeAsset").FirstOrDefault());
            }

            AssetUtilities.LoadIfNull(ref s_VisualTreeAsset, s_VisualTreeAssetPath);

            if (s_VisualTreeAsset == null) {
                Debug.LogError("Could not load VisualTreeAsset for DialogueNodeElement");
                return;
            }

            s_VisualTreeAsset.CloneTree(this);

            m_TextField = this.Q<TextField>("text-field");
            m_OptionsScrollView = this.Q<ScrollView>("dialogue-node-options");
            m_UpdateChecker = this.Q<IMGUIContainer>("dialogue-node__update-checker");

            Foldout foldout = this.Q<Foldout>("dialogue-node-options__foldout");
            Button addOptionButton = new(AddDialogueOption) { text = "+", tooltip = "Add Option", name = "dialogue-node-options__add-option" };
            foldout.Q<Toggle>().Add(addOptionButton);

            this.Q<Button>("dialogue-node__remove-button").clicked += RemoveDialogueNode;

            m_LastCollectionCount = -1;
        }

        public void BindToNode(SerializedProperty dialogueNodeProperty, Dialogue dialogue, DialogueNode node) {
            m_Dialogue = dialogue;
            m_DialogueNode = node;

            m_LastCollectionCount = m_DialogueNode.Options.Count;
            m_UpdateChecker.onGUIHandler = () => {
                if (m_LastCollectionCount == m_DialogueNode.Options.Count) {
                    return;
                }

                m_LastCollectionCount = m_DialogueNode.Options.Count;
                RebuildOptions();
            };

            SerializedProperty textProperty = dialogueNodeProperty.FindPropertyRelative(DialogueNode.TEXT_FIELD_NAME);
            m_TextField.BindProperty(textProperty);
            RebuildOptions();

            Undo.undoRedoPerformed -= RebuildOptions;
            Undo.undoRedoPerformed += RebuildOptions;
        }

        private void AddDialogueOption() {
            Undo.RegisterCompleteObjectUndo(m_Dialogue, "Add Dialogue Option");
            m_DialogueNode.Options.Add(new DialogueOption());
        }

        private void RemoveDialogueNode() {
            Undo.RegisterCompleteObjectUndo(m_Dialogue, "Remove Dialogue Node");
            m_Dialogue.Nodes.Remove(m_DialogueNode);
        }

        private void RebuildOptions() {
            m_OptionsScrollView.Clear();

            int index = 0;
            foreach (DialogueOption option in m_DialogueNode.Options) {
                DialogueOptionElement dialogueOptionElement = new();
                dialogueOptionElement.Bind(m_Dialogue, m_DialogueNode, option);
                m_OptionsScrollView.Add(dialogueOptionElement);

                index++;
                if (index == m_DialogueNode.Options.Count) {
                    dialogueOptionElement.AddToClassList("last");
                }
            }
        }
    }
}