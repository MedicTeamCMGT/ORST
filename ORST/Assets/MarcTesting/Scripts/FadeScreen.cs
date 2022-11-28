using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    public bool fadeonStart = true;
    public float fadeDur = 2;
    public Color fadeColor;
    public Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        if(fadeonStart)
        FadeIn();
    }
    public void FadeIn()
    {
        Fade(1, 0);
    }

    public void FadeOut()
    {
        Fade(0, 1);
    }
 



    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
{
    float timer = 0;
    while (timer<= fadeDur) 
    {
        Color newColor = fadeColor;
        newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer/fadeDur);

        rend.material.SetColor("_Color", newColor);
        timer += Time.deltaTime;
        yield return null;
    }
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;

        rend.material.SetColor("_Color", newColor2);

}

}
