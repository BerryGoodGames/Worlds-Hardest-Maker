using System.Collections;
using MyBox;
using UnityEngine;
using VContainer;

public class AnchorBlockIndexInputEditManager : MonoBehaviour
{
    public static AnchorBlockIndexInputEditManager Instance { get; private set; }
    
    [SerializeField] [ReadOnly] private bool isEditing;
    [SerializeField] [ReadOnly] private AnchorBlockIndexInputController currentEditedIndexInput;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween toolbarTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween infobarEditTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private BarTween playButtonTween;
    
    [Inject] private ISelectionStateService selectionStateService;
    
    public void StartIndexInputEdit(AnchorBlockIndexInputController indexInput)
    {
        currentEditedIndexInput = indexInput;
        StartCoroutine(EditCoroutine());
    }
    
    private void OnStartIndexEdit()
    {
        isEditing = true;
        
        // block menu from opening
        MenuManager.Instance.BlockMenu = true;
        
        // disable panels
        toolbarTween.SetPlay(true);
        infobarEditTween.SetPlay(true);
        playButtonTween.TweenToY(-125, false);
    }
    
    private void OnEndIndexEdit()
    {
        if (!isEditing) return;
        
        isEditing = false;
        currentEditedIndexInput = null;
        
        // release menu
        MenuManager.Instance.BlockMenu = false;
        
        // show panels
        toolbarTween.SetPlay(LevelSessionEditManager.Instance.Playing);
        infobarEditTween.SetPlay(LevelSessionEditManager.Instance.Playing);
        playButtonTween.SetPlay(LevelSessionEditManager.Instance.Playing);
    }
    
    private IEnumerator EditCoroutine()
    {
        if (currentEditedIndexInput == null) yield break;
        
        OnStartIndexEdit();
        
        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0) || !AnchorBlockManager.IsAnyBlockHovered(true))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || selectionStateService.IsSelecting || LevelSessionEditManager.Instance.Playing)
            {
                OnEndIndexEdit();
                yield break;
            }
            
            yield return null;
        }
        
        // apply index to index input
        Instance.currentEditedIndexInput.SetIndexValue(AnchorBlockManager.Instance.HoveredBlockIndex);
        
        Instance.OnEndIndexEdit();
        
        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0)) yield return null;
    }
    
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}