using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySneeze : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private AudioSource audioSource;
    public void Particles()
    {
        particleSystem ??= GetComponent<ParticleSystem>();
        audioSource ??= GetComponent<AudioSource>();

        particleSystem.Play();
        audioSource.Play();
    }
}
