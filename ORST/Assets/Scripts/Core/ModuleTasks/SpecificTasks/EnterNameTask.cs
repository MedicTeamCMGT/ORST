using DG.Tweening;
using ORST.Core.Placeholders;
using ORST.Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.ModuleTasks.SpecificTasks {
    public class EnterNameTask : ModuleTask {
        [SerializeField, Required] private EnterNameUI m_EnterNameUI;
        [SerializeField, Required] private CanvasGroup m_EnterNameCanvasGroup;
        [SerializeField, Required] private GameObject m_EnterNameRoot;

        private bool m_Completed;

        private void Awake() {
            m_EnterNameRoot.gameObject.SetActive(false);
            m_EnterNameCanvasGroup.alpha = 0.0f;
            m_EnterNameUI.NameConfirmed += OnNameConfirmed;
        }

        protected override void OnModuleTaskStarted() {
            base.OnModuleTaskStarted();

            m_EnterNameRoot.gameObject.SetActive(true);
            m_EnterNameCanvasGroup.DOFade(1.0f, 0.5f);
        }

        protected override ModuleTaskState ExecuteModuleTask() {
            return m_Completed ? ModuleTaskState.Successful : ModuleTaskState.Running;
        }

        private void OnNameConfirmed(string playerName) {
            m_Completed = true;
            PlaceholderManager.AddPlaceholder("{PLAYER_NAME}", playerName);

            m_EnterNameCanvasGroup.blocksRaycasts = false;
            m_EnterNameCanvasGroup.DOFade(0.0f, 0.5f).OnComplete(() => {
                m_EnterNameRoot.gameObject.SetActive(false);
            });
        }
    }
}