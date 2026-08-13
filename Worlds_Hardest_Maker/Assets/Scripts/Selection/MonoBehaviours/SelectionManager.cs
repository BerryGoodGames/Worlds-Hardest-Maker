using UnityEngine;
using VContainer;

public class SelectionManager : MonoBehaviour
{
    private EventBus eventBus;
    private ISelectionStateService selectionStateService;
    private ISelectionAreaProvider selectionAreaProvider;
    private IAreaFillService fillService;
    private IAreaErasureService erasureService;

    [SerializeField] private SelectionOptionsPanelController optionsPanelController;
    [Space] [SerializeField] private SelectionPreviewController previewController;
    [Space] [SerializeField] private SelectionOutlineController outlineController;

    [Inject]
    private void Construct(IObjectResolver diContainer, 
        EventBus eventBus, 
        ISelectionStateService selectionStateService, 
        ISelectionAreaProvider selectionAreaProvider,
        IAreaFillService fillService,
        IAreaErasureService erasureService)
    {
        this.eventBus = eventBus;
        this.selectionStateService = selectionStateService;
        this.selectionAreaProvider = selectionAreaProvider;
        this.fillService = fillService;
        this.erasureService = erasureService;

        optionsPanelController.SetEventBus(eventBus);
        previewController.Initialize(eventBus, diContainer, selectionAreaProvider);
        outlineController.SetEventBus(eventBus);
        
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => selectionStateService.ClearSelection();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => selectionStateService.ClearSelection();
    
    private void OnDestroy()
    {
        optionsPanelController.Dispose();
        previewController.Dispose();
        outlineController.Dispose();
        
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
    }
    
    public void OnEraseClicked()
    {
        erasureService.EraseArea(selectionAreaProvider.GetArea());
        selectionStateService.ClearSelection();
    }
    
    public void OnFillClicked()
    {
        fillService.FillArea(selectionAreaProvider.GetArea(), LevelSessionEditManager.Instance.CurrentEditMode);
        selectionStateService.ClearSelection();
    }
    
    public void OnCopyClicked()
    {
        SelectionArea selectedArea = selectionAreaProvider.GetArea();
        CopyManager.Instance.Copy(selectedArea);
        
        selectionStateService.ClearSelection();
    }
    
    public void OnCutClicked()
    {
        SelectionArea selectedArea = selectionAreaProvider.GetArea();
        CopyManager.Instance.Copy(selectedArea);        
        
        erasureService.EraseArea(selectionAreaProvider.GetArea());

        selectionStateService.ClearSelection();
    }
    
    public void OnCancelClicked()
    {
        selectionStateService.CancelSelection();
        selectionStateService.ClearSelection();
    }
}