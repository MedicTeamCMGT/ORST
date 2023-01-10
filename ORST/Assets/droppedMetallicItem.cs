using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

public class droppedMetallicItem : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if(collision.relativeVelocity.magnitude > 1){
            JSAM.AudioManager.PlaySound(ORST_SFXSounds.INT_grabmetal_object_mixed);
        }
    }
}
