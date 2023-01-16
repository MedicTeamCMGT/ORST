using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTap : MonoBehaviour
{

    public Animator Tap;
    public GameObject openText;
    public GameObject closeText;

    public ParticleSystem RunningWater;

    public AudioSource openSound;

    private bool inReach;
    private bool isOpen;
    private bool isClosed;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && isClosed)
        {
            inReach = true;
            openText.SetActive(true);
        }

        if (other.gameObject.tag == "Reach" && isOpen)
        {
            inReach = true;
            closeText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
            closeText.SetActive(false);
        }
    }

    void Start()
    {
        inReach = false;
        isClosed = true;
        isOpen = false;
        closeText.SetActive(false);
        openText.SetActive(false);
        RunningWater.Stop();
    }

    void Update()
    {

        if (inReach && isClosed && Input.GetButtonDown("Interact"))
        {
            Tap.SetBool("Open", true);
            Tap.SetBool("Closed", false);
            openText.SetActive(false);
            openSound.Play();
            isOpen = true;
            isClosed = false;
            RunningWater.Play();
        }

        else if (inReach && isOpen && Input.GetButtonDown("Interact"))
        {
            Tap.SetBool("Open", false);
            Tap.SetBool("Closed", true);
            closeText.SetActive(false);
            openSound.Pause();
            isClosed = true;
            isOpen = false;
            RunningWater.Stop();
        }
    }
}
