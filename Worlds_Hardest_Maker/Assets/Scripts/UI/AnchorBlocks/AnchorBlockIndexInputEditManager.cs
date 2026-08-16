using System.Collections;
using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

public class AnchorBlockIndexInputEditManager : MonoBehaviour
{
    public static AnchorBlockIndexInputEditManager Instance { get; private set; }
    
    [SerializeField] [ReadOnly] private bool isEditing;
    [SerializeField] [ReadOnly] private AnchorBlockIndexInputController currentEditedIndexInput;
    
    [Inject] private ISelectionStateService selectionStateService;
    [Inject] private IEditModeUIBlockerService uiBlockerService;
    
    public void StartIndexInputEdit(AnchorBlockIndexInputController indexInput)
    {
        currentEditedIndexInput = indexInput;
        StartCoroutine(EditCoroutine());
    }
    
    private void OnStartIndexEdit()
    {
        isEditing = true;
        
        uiBlockerService.BlockAndDisable();
    }
    
    private void OnEndIndexEdit()
    {
        if (!isEditing) return;
        
        isEditing = false;
        currentEditedIndexInput = null;
        
        uiBlockerService.ReleaseAndShow();
    }
    
    private IEnumerator EditCoroutine()
    {
        if (currentEditedIndexInput == null) yield break;
        
        OnStartIndexEdit();
        
        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0) || !AnchorBlockManager.Instance.IsAnyBlockHovered(true))
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