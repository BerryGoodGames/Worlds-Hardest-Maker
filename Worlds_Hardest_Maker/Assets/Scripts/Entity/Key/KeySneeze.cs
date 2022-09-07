using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySneeze : MonoBehaviour
{
    public void Particles()
    {
        GetComponent<ParticleSystem>().Play();
    }
}
