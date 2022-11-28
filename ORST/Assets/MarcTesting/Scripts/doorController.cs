using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour
{
    Animator doorAnim;

public void Start() {
    doorAnim = GetComponent<Animator>();
}
public void triggerDoor()
    {
        doorAnim.SetBool("isOpened", true);
    }


}
