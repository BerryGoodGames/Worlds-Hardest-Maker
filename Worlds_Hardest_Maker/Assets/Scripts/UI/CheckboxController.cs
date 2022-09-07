using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
        anim.SetBool("IsOn", GetComponent<Toggle>().isOn);
    }
    public void PlayAnimation(bool enabled)
    {
        if(anim != null)
        {
            anim.SetBool("IsOn", enabled);
        }
    }

    private void OnEnable()
    {
        anim.SetBool("IsOn", GetComponent<Toggle>().isOn);
    }
}
