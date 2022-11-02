using DG.Tweening;
using ORST.Foundation.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Core.UI {
    public class MainMenuPage : BaseMonoBehaviour {
        [SerializeField, Required] private CanvasGroup m_CanvasGroup;

        public CanvasGroup CanvasGroup => m_CanvasGroup;

        public void Hide(float duration) {
            m_CanvasGroup.DOKill();
            m_CanvasGroup.DOFade(0.0f, duration);
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void Show(float duration) {
            m_CanvasGroup.DOKill();
            m_CanvasGroup.DOFade(1.0f, duration);
            m_CanvasGroup.blocksRaycasts = true;
        }
    }
}