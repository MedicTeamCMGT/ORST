using System.Collections.Generic;
using System.IO;
using System.Linq;
using ORST.Core.Dialogues;
using ORST.Core.Editor.UIElements;
using ORST.Core.Editor.Utilities;
using ORST.Foundation.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ORST.Core.Editor.Dialogues {
    public class DialogueEditorWindow : EditorWindow {
        [MenuItem("ORST/Dialogue Editor")]
        private static void ShowWindow() {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.Show();
        }

        [MenuItem("ORST/Destroy all Dialogue Editor")]
        private static void DestroyAllWindows() {
            DialogueEditorWindow[] windows = Resources.FindObjectsOfTypeAll<DialogueEditorWindow>();
            foreach (DialogueEditorWindow window in windows) {
                try {
                    window.Close();
                } catch {
                    DestroyImmediate(window);
                }
            }
        }

        private static string s_WindowTreeAssetPath;
        private static VisualTreeAsset s_WindowTreeAsset;

        private Foldout m_NpcFoldout;
        private Foldout m_DialoguesFoldout;
        private RadioButtonGroup m_NpcGroup;
        private RadioButtonGroup m_DialoguesGroup;
        private VisualElement m_Sidebar;
        private Dragger m_DragArea;
        private VisualElement m_ContentContainer;

        private float m_SidebarWidthBeforeDrag;
        private List<DialogueNPC> m_NPCs;
        private List<Dialogue> m_Dialogues;

        private int m_SelectedNPCIndex = -1;
        private int m_SelectedDialogueIndex = -1;
        private int m_LastDialogueNodeCount = -1;

        private void CreateGUI() {
            if (string.IsNullOrEmpty(s_WindowTreeAssetPath)) {
                s_WindowTreeAssetPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DialogueEditorWindow t:VisualTreeAsset").FirstOrDefault());
            }

            AssetUtilities.LoadIfNull(ref s_WindowTreeAsset, s_WindowTreeAssetPath);

            s_WindowTreeAsset.CloneTree(rootVisualElement);

            m_NpcFoldout = rootVisualElement.Q<Foldout>("npc-container");
            m_NpcFoldout.AddManipulator(new ContextualMenuManipulator(evt => evt.menu.AppendAction("Reload", _ => ReloadNPCAssets())));
            Button createNPCButton = new(CreateNPCAsset) { text = "+", name = "create-npc-button" };
            createNPCButton.AddToClassList("create-button");
            m_NpcFoldout.Q<Toggle>().Add(createNPCButton);

            m_DialoguesFoldout = rootVisualElement.Q<Foldout>("dialogues-container");
            m_DialoguesFoldout.AddManipulator(new ContextualMenuManipulator(evt => evt.menu.AppendAction("Reload", _ => ReloadDialogueAssets())));
            Button createDialogueButton = new(CreateDialogueAsset) { text = "+", name = "create-dialogue-button" };
            createDialogueButton.AddToClassList("create-button");
            m_DialoguesFoldout.Q<Toggle>().Add(createDialogueButton);

            m_NpcGroup = rootVisualElement.Q<RadioButtonGroup>("npc-radio-group");
            m_NpcGroup.RegisterValueChangedCallback(evt => {
                OnNpcSelected(evt.newValue);

                if (evt.newValue == -1) {
                    return;
                }

                m_DialoguesGroup.SetValueWithoutNotify(-1);
            });

            m_DialoguesGroup = rootVisualElement.Q<RadioButtonGroup>("dialogues-radio-group");
            m_DialoguesGroup.RegisterValueChangedCallback(evt => {
                OnDialogueSelected(evt.newValue);

                if (evt.newValue == -1) {
                    return;
                }

                m_NpcGroup.SetValueWithoutNotify(-1);
            });

            m_Sidebar = rootVisualElement.Q("sidebar");
            float sidebarWidth = EditorPrefs.GetFloat("ORST.DialogueEditorWindow.SidebarWidth", 250.0f);
            m_Sidebar.style.width = sidebarWidth;
            m_DragArea = rootVisualElement.Q<Dragger>("drag-area");
            m_DragArea.DragStarted += OnSidebarResizeStarted;
            m_DragArea.DragUpdated += OnSidebarResizeUpdated;

            m_ContentContainer = rootVisualElement.Q("content-container");

            ReloadNPCAssets();
            ReloadDialogueAssets();
        }

        private void ReloadNPCAssets() {
            m_NPCs = AssetDatabase.FindAssets("t:DialogueNPC")
                                  .Select(AssetDatabase.GUIDToAssetPath)
                                  .Select(AssetDatabase.LoadAssetAtPath<DialogueNPC>)
                                  .ToList();
            m_NpcGroup.choices = m_NPCs.Select(npc => string.IsNullOrEmpty(npc.Name) ? "Unnamed NPC" : npc.Name);

            int npcIndex = 0;
            foreach (VisualElement visualElement in m_NpcGroup.Q(className: "unity-base-field__input").Children()) {
                if (visualElement is not RadioButton radioButton) {
                    continue;
                }

                radioButton.userData = m_NPCs[npcIndex];
                radioButton.AddToClassList("dialogue-radio-button");
                radioButton.AddManipulator(new ContextualMenuManipulator(evt => {
                    evt.menu.AppendAction("Delete", _ => {
                        if (!EditorUtility.DisplayDialog("Delete NPC", "Are you sure you want to delete this NPC?", "Yes", "No")) {
                            return;
                        }

                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath((DialogueNPC)radioButton.userData));
                        m_ContentContainer.Clear();
                        ReloadNPCAssets();
                    });
                }));

                npcIndex++;
            }

            if (m_SelectedNPCIndex >= m_NPCs.Count) {
                m_SelectedNPCIndex = -1;
            }

            if (m_SelectedNPCIndex != -1) {
                m_ContentContainer.Clear();
                m_NpcGroup.SetValueWithoutNotify(-1);
                m_NpcGroup.value = m_SelectedNPCIndex;
            }
        }

        private void ReloadDialogueAssets() {
            m_Dialogues = AssetDatabase.FindAssets("t:Dialogue")
                                       .Select(AssetDatabase.GUIDToAssetPath)
                                       .Select(AssetDatabase.LoadAssetAtPath<Dialogue>)
                                       .ToList();
            m_DialoguesGroup.choices = m_Dialogues.Select(dialogue => dialogue.name);

            int dialogueIndex = 0;
            foreach (VisualElement visualElement in m_DialoguesGroup.Q(className: "unity-base-field__input").Children()) {
                if (visualElement is not RadioButton radioButton) {
                    continue;
                }

                radioButton.userData = m_Dialogues[dialogueIndex];
                radioButton.AddToClassList("dialogue-radio-button");
                radioButton.AddManipulator(new ContextualMenuManipulator(evt => {
                    evt.menu.AppendAction("Delete", _ => {
                        if (!EditorUtility.DisplayDialog("Delete Dialogue", "Are you sure you want to delete this dialogue?", "Yes", "No")) {
                            return;
                        }

                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath((Dialogue)radioButton.userData));
                        m_ContentContainer.Clear();
                        ReloadDialogueAssets();
                    });
                }));

                dialogueIndex++;
            }

            if (m_SelectedDialogueIndex >= m_Dialogues.Count) {
                m_SelectedDialogueIndex = -1;
            }

            if (m_SelectedDialogueIndex != -1) {
                m_ContentContainer.Clear();
                m_DialoguesGroup.SetValueWithoutNotify(-1);
                m_DialoguesGroup.value = m_SelectedDialogueIndex;
            }
        }

        private void OnSidebarResizeStarted() {
            m_SidebarWidthBeforeDrag = m_Sidebar.resolvedStyle.width;
        }

        private void OnSidebarResizeUpdated(Vector2 initialPosition, Vector2 currentPosition) {
            Vector2 delta = currentPosition - initialPosition;
            float newWidth = m_SidebarWidthBeforeDrag + delta.x;
            EditorPrefs.SetFloat("ORST.DialogueEditorWindow.SidebarWidth", newWidth);
            m_Sidebar.style.width = Mathf.Max(m_Sidebar.resolvedStyle.minWidth.value, newWidth);
        }

        private void OnNpcSelected(int npcIndex) {
            m_SelectedNPCIndex = npcIndex;
            m_SelectedDialogueIndex = -1;
            m_ContentContainer.Clear();

            if (npcIndex < 0 || npcIndex >= m_NPCs.Count) {
                return;
            }

            DialogueNPC npc = m_NPCs[npcIndex];
            VisualElement npcEditor = CreateNPCEditor(npc, npcIndex);
            m_ContentContainer.Add(npcEditor);
        }

        private VisualElement CreateNPCEditor(DialogueNPC npc, int npcIndex) {
            SerializedObject serializedObject = new(npc);
            serializedObject.Update();

            VisualElement root = new();
            root.AddToClassList("npc-editor");

            VisualElement headerContainer = new() {name = "npc-editor-header"};
            root.Add(headerContainer);
            SerializedProperty iconProperty = serializedObject.FindProperty("m_Icon");
            PropertyField iconField = new(iconProperty, string.Empty);
            iconField.AddToClassList("npc-editor__icon-field");
            iconField.Bind(serializedObject);
            iconField.BindProperty(iconProperty);
            iconField.RegisterValueChangeCallback(_ => iconField.Q<Image>().image = npc.Icon.OrNull()?.texture);
            iconField.schedule.Execute(
                () => iconField.schedule.Execute(
                    () => iconField.Q<Image>().scaleMode = ScaleMode.ScaleToFit
                )
            );
            headerContainer.Add(iconField);

            SerializedProperty nameProperty = serializedObject.FindProperty("m_NPCName");
            PropertyField nameField = new(nameProperty, string.Empty);
            nameField.AddToClassList("npc-editor__name-field");
            nameField.Bind(serializedObject);
            nameField.BindProperty(nameProperty);
            nameField.RegisterValueChangeCallback(evt => {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                string newName = npc.Name;
                if (string.IsNullOrEmpty(newName)) {
                    newName = "Unnamed NPC";
                }

                m_NpcGroup.Q(className: "unity-base-field__input").Children().Skip(npcIndex).First().Q<Label>().text = newName;
            });
            headerContainer.Add(nameField);

            VisualElement settingsContainer = new();
            root.Add(settingsContainer);

            SerializedProperty roleProperty = serializedObject.FindProperty("m_Role");
            PropertyField roleField = new(roleProperty);
            roleField.AddToClassList("npc-editor__role-field");
            roleField.AddToClassList("dialogue-editor__text-field");
            roleField.Bind(serializedObject);
            roleField.BindProperty(roleProperty);
            settingsContainer.Add(roleField);

            return root;
        }

        private void CreateNPCAsset() {
            string basePath = m_NPCs.Count > 0 ? Path.GetDirectoryName(AssetDatabase.GetAssetPath(m_NPCs[0])) : "Assets";
            string filePath = EditorUtility.SaveFilePanelInProject("Save NPC", "New NPC", "asset", "Save NPC", basePath);

            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            DialogueNPC npc = CreateInstance<DialogueNPC>();
            AssetDatabase.CreateAsset(npc, filePath);
            AssetDatabase.SaveAssets();

            rootVisualElement.schedule.Execute(() => {
                ReloadNPCAssets();
                // Find index of NPC with asset path
                int index = -1;
                for (int i = 0; i < m_NPCs.Count; i++) {
                    if (AssetDatabase.GetAssetPath(m_NPCs[i]) == filePath) {
                        index = i;
                        break;
                    }
                }

                m_NpcGroup.value = index;
            }).ExecuteLater(30);
        }

        private void OnDialogueSelected(int dialogueIndex) {
            m_SelectedDialogueIndex = dialogueIndex;
            m_SelectedNPCIndex = -1;
            m_ContentContainer.Clear();

            if (dialogueIndex < 0 || dialogueIndex >= m_Dialogues.Count) {
                return;
            }

            Dialogue dialogue = m_Dialogues[dialogueIndex];
            VisualElement dialogueEditor = CreateDialogueEditor(dialogue, dialogueIndex);
            m_ContentContainer.Add(dialogueEditor);
        }

        private VisualElement CreateDialogueEditor(Dialogue dialogue, int dialogueIndex) {
            SerializedObject serializedObject = new(dialogue);
            serializedObject.Update();

            VisualElement root = new();
            root.AddToClassList("dialogue-editor");

            Label titleLabel = new() {
                text = dialogue.name
            };
            titleLabel.AddToClassList("dialogue-editor__title");
            root.Add(titleLabel);

            SerializedProperty nodesProperty = serializedObject.FindProperty(Dialogue.NODES_FIELD_NAME);
            Foldout nodesFoldout = new() { text = "Nodes", name = "dialogue-editor__nodes-foldout" };
            nodesFoldout.AddToClassList("dialogue-foldout");

            Button addNodeButton = new(() => {
                Undo.RegisterCompleteObjectUndo(dialogue, "Add Dialogue Node");
                dialogue.Nodes.Add(new DialogueNode());
            }) { text = "+", tooltip = "Add Node", name = "dialogue-editor__add-node" };
            nodesFoldout.Q<Toggle>().Add(addNodeButton);


            ScrollView nodesContainer = new();
            nodesContainer.AddToClassList("dialogue-editor__nodes-list");
            nodesFoldout.Add(nodesContainer);
            m_LastDialogueNodeCount = dialogue.Nodes.Count;
            IMGUIContainer nodesUpdateChecker = new(() => {
                if (dialogue.Nodes.Count == m_LastDialogueNodeCount) {
                    return;
                }

                m_LastDialogueNodeCount = dialogue.Nodes.Count;
                UpdateDialogueNodes(dialogue, nodesContainer, nodesProperty);
            });

            nodesUpdateChecker.cullingEnabled = false;
            nodesFoldout.Add(nodesUpdateChecker);
            root.Add(nodesFoldout);

            UpdateDialogueNodes(dialogue, nodesContainer, nodesProperty);

            return root;
        }

        private static void UpdateDialogueNodes(Dialogue dialogue, ScrollView nodesContainer, SerializedProperty nodesProperty) {
            nodesContainer.Clear();
            nodesProperty.serializedObject.Update();
            int index = 0;
            foreach (DialogueNode dialogueNode in dialogue.Nodes) {
                if (index >= 0 && index < nodesProperty.arraySize) {
                    DialogueNodeElement nodeElement = new();
                    nodeElement.BindToNode(nodesProperty.GetArrayElementAtIndex(index++), dialogue, dialogueNode);
                    nodesContainer.Add(nodeElement);
                } else {
                    Debug.LogError($"Dialogue node count mismatch: {dialogue.Nodes.Count} != {nodesProperty.arraySize}");
                }
            }
        }

        private void CreateDialogueAsset() {
            string basePath = m_Dialogues.Count > 0 ? Path.GetDirectoryName(AssetDatabase.GetAssetPath(m_Dialogues[0])) : "Assets";
            string filePath = EditorUtility.SaveFilePanelInProject("Save Dialogue", "New Dialogue", "asset", "Save Dialogue", basePath);

            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            Dialogue dialogue = CreateInstance<Dialogue>();
            AssetDatabase.CreateAsset(dialogue, filePath);
            AssetDatabase.SaveAssets();

            rootVisualElement.schedule.Execute(() => {
                ReloadDialogueAssets();

                // Find index of dialogue with asset path
                int index = -1;
                for (int i = 0; i < m_Dialogues.Count; i++) {
                    if (AssetDatabase.GetAssetPath(m_Dialogues[i]) == filePath) {
                        index = i;
                        break;
                    }
                }

                m_DialoguesGroup.value = index;
            }).ExecuteLater(30);
        }
    }
}