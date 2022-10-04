using System.Collections.Generic;
using ORST.Core.Editor.UIElements;
using UnityEditor;
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

        private RadioButtonGroup m_NpcGroup;
        private RadioButtonGroup m_DialoguesGroup;
        private VisualElement m_Sidebar;
        private Dragger m_DragArea;
        private VisualElement m_ContentContainer;

        private float m_SidebarWidthBeforeDrag;

        private void CreateGUI() {
            if (s_windowTreeAsset == null) {
                InitializeResources();
            }

            s_windowTreeAsset.CloneTree(rootVisualElement);

            m_NpcGroup = rootVisualElement.Q<RadioButtonGroup>("npc-radio-group");
            m_NpcGroup.choices = new List<string> { "Hello world", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum", "Yeeet", "Lorem ipsum" };
            foreach (VisualElement visualElement in m_NpcGroup.Q(className: "unity-base-field__input").Children()) {
                if (visualElement is not RadioButton radioButton) {
                    continue;
                }

                radioButton.AddToClassList("dialogue-radio-button");
            }

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
            Debug.Log($"Npc selected: {npcIndex}");
        }

        private void OnDialogueSelected(int dialogueIndex) {
            Debug.Log($"Dialogue selected: {dialogueIndex}");
        }
    }
}