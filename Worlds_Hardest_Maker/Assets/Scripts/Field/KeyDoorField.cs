using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorField : MonoBehaviour
{
    public bool unlocked = false;
    public MKey.KeyColor color;

    public void Lock(bool locked)
    {
        unlocked = !locked;

        GetComponent<BoxCollider2D>().enabled = locked;

        Animator anim = GetComponent<Animator>();
        anim.SetBool("Unlocked", !locked);
    }
}
