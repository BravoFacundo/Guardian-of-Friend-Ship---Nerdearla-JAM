using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnabled : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        audioSource.Play();
    }
}
