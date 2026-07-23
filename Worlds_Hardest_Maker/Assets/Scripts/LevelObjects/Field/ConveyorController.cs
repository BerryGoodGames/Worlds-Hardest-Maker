using UnityEngine;
using Zenject;

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
            eventBus.Subscribe<SwitchToPlayEvent>(_ => SwitchAnimToRunning());
            eventBus.Subscribe<SwitchToEditEvent>(_ => SwitchAnimToStaying());
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
        eventBus.Unsubscribe<SwitchToPlayEvent>(_ => SwitchAnimToRunning());
        eventBus.Unsubscribe<SwitchToEditEvent>(_ => SwitchAnimToStaying());
    }
}