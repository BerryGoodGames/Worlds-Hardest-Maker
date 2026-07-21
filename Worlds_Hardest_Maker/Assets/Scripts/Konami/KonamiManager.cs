using NaughtyAttributes;
using UnityEngine;
using Zenject;

/// <Summary>
///     Checks inputted key if it's the konami Code
/// </Summary>
public class KonamiManager : MonoBehaviour, IKonamiService
{
    public bool IsKonamiActive { get; private set; }
    
    private EventBus eventBus;
    
    [SerializeField] [Required] private KonamiCodeActivationAnimation activationAlert;
    
    private int keyIndex;
    
    // Konami Code: up up down down left right left right BA
    private readonly KeyCode[] konamiKeys =
    {
        KeyCode.UpArrow, KeyCode.UpArrow,
        KeyCode.DownArrow, KeyCode.DownArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.B, KeyCode.A,
    };
    
    [Inject]
    private void Construct(EventBus eventBus)
    {
        this.eventBus = eventBus;
    }
    
    private void Update()
    {
        if (!Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2)) return;
        
        // detect konami code
        if (Input.GetKeyDown(konamiKeys[keyIndex]) && !activationAlert.IsAnimationOnScreen)
        {
            keyIndex++;
            
            // check if code is finished
            if (keyIndex < konamiKeys.Length) return;
            
            SetKonamiActive(!IsKonamiActive);
            
            print($"Konami {(IsKonamiActive ? "en" : "dis")}abled");
            keyIndex = 0;
        }
        else keyIndex = 0;
    }
    
    private void SetKonamiActive(bool active)
    {
        IsKonamiActive = active;
        
        eventBus.Fire(new KonamiStateChangedEvent(active));
    }
}