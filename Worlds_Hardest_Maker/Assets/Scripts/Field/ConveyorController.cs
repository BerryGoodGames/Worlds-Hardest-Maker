using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private FieldRotation rotationController;
    private Animator anim;
    [SerializeField] private float strength;
    public float Strength { get { return strength; } set { strength = value; } }
    public float Rotation { get { return transform.rotation.z; } }

    public void Rotate()
    {
        rotationController.StartRotation();
    }

    private void Start()
    {
        rotationController = GetComponent<FieldRotation>();
        anim = GetComponent<Animator>();

        GameManager.onPlay += SwitchAnimToRunning;
        GameManager.onEdit += SwitchAnimToStaying;
    }

    private void SwitchAnimToRunning()
    {
        if (anim == null)
            GetComponent<Animator>();
        anim.SetBool("Running", true);
    }

    private void SwitchAnimToStaying()
    {
        if (anim == null)
            GetComponent<Animator>();
        anim.SetBool("Running", false);
    }

    private void OnDestroy()
    {
        GameManager.onPlay -= SwitchAnimToRunning;
        GameManager.onEdit -= SwitchAnimToStaying;
    }
}
