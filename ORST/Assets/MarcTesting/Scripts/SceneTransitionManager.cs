using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    Animator doorAnim;

    public void Start() 
    {
        if(GameObject.FindGameObjectWithTag("Transi"))
        {
        doorAnim = GameObject.FindGameObjectWithTag("Transi").GetComponent<Animator>();
        } 
    }
    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();

        //yield return WaitForSeconds(fadeScreen.fadeDur);

        //launch scene

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        if(GameObject.FindGameObjectWithTag("Transi"))
        {
        doorAnim.SetBool("isOpened", true);
        }
        
        float timer = 0;
        while(timer<= fadeScreen.fadeDur && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        operation.allowSceneActivation = true;
        doorAnim = null;
    }


}
