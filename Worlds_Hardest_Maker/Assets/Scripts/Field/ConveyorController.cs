using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private FieldRotation rotationController;
    private Animator anim;
    private float animSpeed;

    [SerializeField] private float strength;

    private static readonly int Running = Animator.StringToHash("Running");

    public float Strength { 
        get => strength;
        set => strength = value;
    }
    public float Rotation => transform.rotation.z;

    public void Rotate()
    {
        rotationController.StartRotation();
    }

    private void Start()
    {
        rotationController = GetComponent<FieldRotation>();
        anim = GetComponent<Animator>();

        GameManager.Instance.OnPlay += SwitchAnimToRunning;
        GameManager.Instance.OnEdit += SwitchAnimToStaying;
    }

    private void SwitchAnimToRunning()
    {
        if (anim == null)
            GetComponent<Animator>();

        anim.speed = Strength;
        anim.SetBool(Running, true);
    }

    private void SwitchAnimToStaying()
    {
        if (anim == null)
            GetComponent<Animator>();
        anim.SetBool(Running, false);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPlay -= SwitchAnimToRunning;
        GameManager.Instance.OnEdit -= SwitchAnimToStaying;
    }
}
