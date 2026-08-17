using MyBox;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using WorldsHardestMaker.CopyPaste;

namespace WorldsHardestMaker.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        private EventBus eventBus;
        private ISelectionStateService selectionStateService;
        private ISelectionAreaProvider selectionAreaProvider;
        private IAreaFillService fillService;
        private IAreaErasureService erasureService;
        private ICopyPasteService copyPasteService;

        [SerializeField] [InitializationField] [MustBeAssigned] private Transform fieldContainer;
        [SerializeField] [InitializationField] [MustBeAssigned] private Transform playerContainer;
        [Space] [SerializeField] private SelectionOptionsPanelController optionsPanelController;
        [Space] [SerializeField] private SelectionPreviewService previewService;
        [Space] [SerializeField] private SelectionOutlineController outlineController;

        [Inject]
        private void Construct(IObjectResolver diContainer, 
            EventBus eventBus, 
            ISelectionStateService selectionStateService, 
            ISelectionAreaProvider selectionAreaProvider,
            IAreaFillService fillService,
            IAreaErasureService erasureService,
            IDrawService drawService,
            ICopyPasteService copyPasteService)
        {
            this.eventBus = eventBus;
            this.selectionStateService = selectionStateService;
            this.selectionAreaProvider = selectionAreaProvider;
            this.fillService = fillService;
            this.erasureService = erasureService;
            this.copyPasteService = copyPasteService;

            optionsPanelController.SetEventBus(eventBus);
            previewService.Initialize(eventBus, diContainer, selectionAreaProvider);
            outlineController.Initialize(eventBus, drawService);
        
            eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
            eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        }
    
        private void OnSwitchToPlay(SwitchToPlayEvent evt) => selectionStateService.ClearSelection();
        private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => selectionStateService.ClearSelection();
    
        private void OnDestroy()
        {
            optionsPanelController.Dispose();
            previewService.Dispose();
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
            fillService.FillArea(selectionAreaProvider.GetArea(), LevelSessionEditManager.Instance.CurrentEditMode, fieldContainer, playerContainer);
            selectionStateService.ClearSelection();
        }
    
        public void OnCopyClicked()
        {
            SelectionArea selectedArea = selectionAreaProvider.GetArea();
            copyPasteService.Copy(selectedArea);
        
            selectionStateService.ClearSelection();
        }
    
        public void OnCutClicked()
        {
            SelectionArea selectedArea = selectionAreaProvider.GetArea();
            copyPasteService.Copy(selectedArea);        
            erasureService.EraseArea(selectedArea);

            selectionStateService.ClearSelection();
        }
    
        public void OnCancelClicked()
        {
            selectionStateService.CancelSelection();
            selectionStateService.ClearSelection();
        }
    }
}