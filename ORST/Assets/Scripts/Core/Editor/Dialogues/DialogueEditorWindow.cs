using System.Collections.Generic;
using System.IO;
using System.Linq;
using ORST.Core.Dialogues;
using ORST.Core.Editor.UIElements;
using ORST.Foundation.Foundation.Extensions;
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

        private static VisualTreeAsset s_windowTreeAsset;

        private static void InitializeResources() {
            s_windowTreeAsset = Resources.Load<VisualTreeAsset>("Dialogues/DialogueEditorWindow");
        }

        private Foldout m_NpcFoldout;
        private Foldout m_DialoguesFoldout;
        private RadioButtonGroup m_NpcGroup;
        private RadioButtonGroup m_DialoguesGroup;
        private VisualElement m_Sidebar;
        private Dragger m_DragArea;
        private VisualElement m_ContentContainer;

        private List<DialogueNPC> m_NPCs;
        private float m_SidebarWidthBeforeDrag;

        private int m_SelectedNPCIndex = -1;
        private int m_SelectedDialogueIndex = -1;

        private void CreateGUI() {
            if (s_windowTreeAsset == null) {
                InitializeResources();
            }

            s_windowTreeAsset.CloneTree(rootVisualElement);

            m_NpcFoldout = rootVisualElement.Q<Foldout>("npc-container");
            m_NpcFoldout.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.AppendAction("Reload", action => { ReloadNPCAssets(); }); }));
            Button createNPCButton = new(CreateNPCAsset) { text = "+", name = "create-npc-button" };
            m_NpcFoldout.Q<Toggle>().Add(createNPCButton);

            m_DialoguesFoldout = rootVisualElement.Q<Foldout>("dialogues-container");

            m_NpcGroup = rootVisualElement.Q<RadioButtonGroup>("npc-radio-group");
            m_NpcGroup.RegisterValueChangedCallback(evt => {
                OnNpcSelected(evt.newValue);

                if (evt.newValue == -1) {
                    return;
                }

                m_DialoguesGroup.SetValueWithoutNotify(-1);
            });

            m_DialoguesGroup = rootVisualElement.Q<RadioButtonGroup>("dialogues-radio-group");
            m_DialoguesGroup.choices = new List<string> { "Hello world", "Yeeet", "Lorem ipsum" };
            foreach (VisualElement visualElement in m_DialoguesGroup.Q(className: "unity-base-field__input").Children()) {
                if (visualElement is not RadioButton radioButton) {
                    continue;
                }

                radioButton.AddToClassList("dialogue-radio-button");
            }

            m_DialoguesGroup.RegisterValueChangedCallback(evt => {
                OnDialogueSelected(evt.newValue);

                if (evt.newValue == -1) {
                    return;
                }

                m_NpcGroup.SetValueWithoutNotify(-1);
            });

            m_Sidebar = rootVisualElement.Q("sidebar");
            m_DragArea = rootVisualElement.Q<Dragger>("drag-area");
            m_DragArea.DragStarted += OnSidebarResizeStarted;
            m_DragArea.DragUpdated += OnSidebarResizeUpdated;

            m_ContentContainer = rootVisualElement.Q("content-container");

            ReloadNPCAssets();
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

        private void OnSidebarResizeStarted() {
            m_SidebarWidthBeforeDrag = m_Sidebar.resolvedStyle.width;
        }

        private void OnSidebarResizeUpdated(Vector2 initialPosition, Vector2 currentPosition) {
            Vector2 delta = currentPosition - initialPosition;
            float newWidth = m_SidebarWidthBeforeDrag + delta.x;
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
        }
    }
}