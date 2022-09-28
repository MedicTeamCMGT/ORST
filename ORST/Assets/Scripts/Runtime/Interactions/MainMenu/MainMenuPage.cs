using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ORST.Runtime.Interactions {
    public class MainMenuPage : SerializedMonoBehaviour {
        [SerializeField, Required] private CanvasGroup m_CanvasGroup;

        public CanvasGroup CanvasGroup => m_CanvasGroup;

        public void Hide(float duration) {
            m_CanvasGroup.DOKill();
            m_CanvasGroup.DOFade(0.0f, duration);
        }

        public void Show(float duration) {
            m_CanvasGroup.DOKill();
            m_CanvasGroup.DOFade(1.0f, duration);
        }
    }
}