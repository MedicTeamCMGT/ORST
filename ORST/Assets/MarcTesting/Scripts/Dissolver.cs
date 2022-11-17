using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    private Material mat;
    public bool hideObject;
    public float dissolveSpeed = 2f;



    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            hideObject = !hideObject;
        }
        if(hideObject){
            mat.SetFloat("_Progress", Mathf.MoveTowards(mat.GetFloat("_Progress"), 1.25f, dissolveSpeed * Time.deltaTime));
        } else {
            
            mat.SetFloat("_Progress", Mathf.MoveTowards(mat.GetFloat("_Progress"), -.25f, dissolveSpeed * Time.deltaTime));
        }

        
    }
}
