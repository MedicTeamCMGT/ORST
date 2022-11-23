using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    public bool hideObject;
    public float dissolveSpeed = 2f;
    public Material dissolveMaterial;
    private float dissolveAmount = 0;


    // Start is called before the first frame update
    void Start()
    {
        dissolveAmount = dissolveMaterial.GetFloat("_DissolveAmount");
        //Material newMat = Resources.Load("Floor mask", typeof(Material)) as Material;
        //mat = this.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            hideObject = !hideObject;
        }
        if(hideObject){
            dissolveAmount = Mathf.MoveTowards(dissolveAmount, 1.25f, dissolveSpeed * Time.deltaTime);
        } else {
            dissolveAmount = Mathf.MoveTowards(dissolveAmount, -.25f, dissolveSpeed * Time.deltaTime);
        }

        dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);

        
    }
}
