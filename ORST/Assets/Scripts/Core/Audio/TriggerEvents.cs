using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class TriggerEvents : MonoBehaviour
{
    private AudioSource _audioSource = null;
    private List<AudioClip> _audioClipPool = new List<AudioClip>();

    [SerializeField] private AudioClip[] _audioClips;

    public void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        
        if(_audioClips.Length > 1) {
            for (int i = 0; i < _audioClips.Length; i++)
        {
            _audioClipPool.Add(_audioClips[i]);
        }
        }

    }
            
        
    public void PlayAudio()
    {
        _audioSource.Play();

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Laser"))
        {
            Debug.Log("Enter");
            PlayAudio();

        }
    }
 
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Laser"))
        {
            Debug.Log("Exit");
        }
    }
 
    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Laser"))
        {
            Debug.Log("Stay");

        }
    }
}