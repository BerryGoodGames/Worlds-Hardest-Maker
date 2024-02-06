using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    private FieldRotation rotationController;
    private Animator anim;
    private static readonly int running = Animator.StringToHash("Running");

    public float Speed => LevelSettings.Instance.ConveyorSpeed;

    public float Rotation => transform.rotation.eulerAngles.z;

    public void Rotate() => rotationController.StartRotation();

    private void Start()
    {
        rotationController = GetComponent<FieldRotation>();
        anim = GetComponent<Animator>();

        if (LevelSessionManager.Instance.IsEdit)
        {
            PlayManager.Instance.OnSwitchToPlay += SwitchAnimToRunning;
            PlayManager.Instance.OnSwitchToEdit += SwitchAnimToStaying;
        }
        else SwitchAnimToRunning();
    }

    public void SwitchAnimToRunning()
    {
        if (anim == null) GetComponent<Animator>();

        anim.speed = Speed;
        anim.SetBool(running, true);
    }

    private void SwitchAnimToStaying()
    {
        if (anim == null) GetComponent<Animator>();

        anim.SetBool(running, false);
    }

    private void OnDestroy()
    {
        PlayManager.Instance.OnSwitchToPlay -= SwitchAnimToRunning;
        PlayManager.Instance.OnSwitchToEdit -= SwitchAnimToStaying;
    }
}