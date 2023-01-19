using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ORST.Core.Utilities {
    public class ORSTSceneLoader : MonoBehaviour {
        private bool m_IsLoadingScene;

        public void LoadSceneFade(int sceneIndex) {
            if (m_IsLoadingScene) {
                return;
            }

            m_IsLoadingScene = true;
            StartCoroutine(LoadSceneCoroutine(sceneIndex, true));
        }

        public void LoadScene(int sceneIndex) {
            if (m_IsLoadingScene) {
                return;
            }

            m_IsLoadingScene = true;
            StartCoroutine(LoadSceneCoroutine(sceneIndex, false));
        }

        private IEnumerator LoadSceneCoroutine(int sceneIndex, bool fadeToBlack) {
            if (fadeToBlack) {
                OVRScreenFade.instance.FadeOut();
                yield return new WaitUntil(() => Mathf.Abs(OVRScreenFade.instance.currentAlpha - 1.0f) < 0.01f);
            }

            SceneManager.LoadScene(sceneIndex);
        }
    }
}