using MyBox;
using UnityEngine;
using VContainer;

/// <summary>
///     Fills wall fields centered around (0, 0), amount based on INTENSITY
///     <para>Attach to new gameObject</para>
/// </summary>
public class DestroyingOurProject : MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    public int INTENSITY;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform fieldContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform playerContainer;
    
    [Inject] private IAreaFillService fillService;
    [Inject] private SaveSystem saveSystem;
    
    private void Start()
    {
        print($"We're about to fill {Mathf.Pow(INTENSITY * 2 + 1, 2)} fields! (gotta go)");
        
        fillService.FillArea(
            new(-INTENSITY, -INTENSITY), new(INTENSITY, INTENSITY),
            EditModeManager.Wall, fieldContainer, playerContainer
        );
        
        saveSystem.SaveCurrentLevel();
    }
}