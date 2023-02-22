using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private FieldRotation rotationController;
    private Animator anim;
    private float animSpeed;

    [SerializeField] private float strength;

    private static readonly int running = Animator.StringToHash("Running");

    public float Strength
    {
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

        EditModeManager.Instance.OnPlay += SwitchAnimToRunning;
        EditModeManager.Instance.OnEdit += SwitchAnimToStaying;
    }

    private void SwitchAnimToRunning()
    {
        if (anim == null)
            GetComponent<Animator>();

        anim.speed = Strength;
        anim.SetBool(running, true);
    }

    private void SwitchAnimToStaying()
    {
        if (anim == null)
            GetComponent<Animator>();
        anim.SetBool(running, false);
    }

    private void OnDestroy()
    {
        EditModeManager.Instance.OnPlay -= SwitchAnimToRunning;
        EditModeManager.Instance.OnEdit -= SwitchAnimToStaying;
    }
}