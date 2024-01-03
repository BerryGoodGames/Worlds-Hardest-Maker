using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private FieldRotation rotationController;
    private Animator anim;
    private static readonly int running = Animator.StringToHash("Running");

    [field: SerializeField] public float Strength { get; set; }

    public float Rotation => transform.rotation.eulerAngles.z;

    public void Rotate() => rotationController.StartRotation();

    private void Start()
    {
        rotationController = GetComponent<FieldRotation>();
        anim = GetComponent<Animator>();

        if (LevelSessionManager.Instance.IsEdit)
        {
            EditModeManagerOther.Instance.OnPlay += SwitchAnimToRunning;
            EditModeManagerOther.Instance.OnEdit += SwitchAnimToStaying;
        }
        else SwitchAnimToRunning();
    }

    public void SwitchAnimToRunning()
    {
        if (anim == null) GetComponent<Animator>();

        anim.speed = Strength;
        anim.SetBool(running, true);
    }

    private void SwitchAnimToStaying()
    {
        if (anim == null) GetComponent<Animator>();

        anim.SetBool(running, false);
    }

    private void OnDestroy()
    {
        EditModeManagerOther.Instance.OnPlay -= SwitchAnimToRunning;
        EditModeManagerOther.Instance.OnEdit -= SwitchAnimToStaying;
    }
}