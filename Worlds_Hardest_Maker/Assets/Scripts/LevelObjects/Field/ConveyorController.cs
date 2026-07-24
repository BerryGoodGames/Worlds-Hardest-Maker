using UnityEngine;
using VContainer;

public class ConveyorController : MonoBehaviour
{
    private Animator anim;
    private static readonly int running = Animator.StringToHash("Running");
    
    public float Speed => LevelSettings.Instance.ConveyorSpeed;
    
    public float Rotation => transform.rotation.eulerAngles.z;
    
    private EventBus eventBus;
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
    }

    private void Start()
    {
        GetComponent<FieldRotation>();
        anim = GetComponent<Animator>();
        
        if (LevelSessionManager.Instance.IsEdit)
        {
            eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
            eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        }
        else SwitchAnimToRunning();
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => SwitchAnimToRunning();
    private void OnSwitchToEdit(SwitchToEditEvent evt) => SwitchAnimToStaying();
    
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
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
}