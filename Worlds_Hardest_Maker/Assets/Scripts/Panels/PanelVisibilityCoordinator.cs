using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using VContainer;

public class PanelVisibilityCoordinator : MonoBehaviour
{
    [SerializeField] private List<PanelVisibilityRule> rules;
    [Separator] [SerializeField] private List<HideableUIElement> managedButtons;
    
    private PanelUIState currentState;
    private readonly PanelUIStateService stateService = new();
    
    [Inject] private EventBus eventBus;
    [Inject] private IPanelService panelService;
    [Inject] private IPanelRegistry panelRegistry;
    
    private void ApplyCurrentState()
    {
        PanelUIState newState = stateService.GetCurrentUIState();
        
        IReadOnlyCollection<IHideableUI> oldVisibleElements = GetVisibleElements(currentState);
        
        IReadOnlyCollection<IHideableUI> newVisibleElements = GetVisibleElements(newState);

        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            // if (!newVisibleElements.Contains(panel))
            // {
            //     panelService.SetPanelHidden(panel, oldVisibleElements.Contains(panel));
            // }

            bool inOld = oldVisibleElements.Contains(panel);
            bool inNew = newVisibleElements.Contains(panel);

            if (inOld && inNew) continue;
            
            panelService.SetPanelHidden(panel, !inNew);
        }

        // foreach (IPanel panel in panelRegistry.RegisteredPanels)
        // {
        //     if (!oldVisibleElements.Contains(panel))
        //     {
        //         panelService.SetPanelHidden(panel, newVisibleElements.Contains(panel));
        //     }
        // }
        
        foreach (HideableUIElement button in managedButtons)
        {
            // if (!newVisibleElements.Contains(button))
            // {
            //     button.SetHidden(oldVisibleElements.Contains(button));
            // }
            
            bool inOld = oldVisibleElements.Contains(button);
            bool inNew = newVisibleElements.Contains(button);

            if (inOld && inNew) continue;
            
            button.SetHidden(!inNew);
        }

        // foreach (HideableUIElement button in managedButtons)
        // {
        //     if (!oldVisibleElements.Contains(button))
        //     {
        //         button.SetHidden(newVisibleElements.Contains(button));
        //     }
        // }

        currentState = newState;
    }
    
    private IReadOnlyCollection<IHideableUI> GetVisibleElements(PanelUIState? state)
    {
        if (state == null)
        {
            return new List<IHideableUI>();
        }

        PanelVisibilityRule rule = rules.Find(r => r.UIState == state.Value);

        if (rule == null)
        {
            return new List<IHideableUI>();
        }

        return rule.VisibleElements.ToList();
    }

    private void RestoreCurrentState()
    {
        IReadOnlyCollection<IHideableUI> visibleElements = GetVisibleElements(currentState);
        
        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            panelService.SetPanelHidden(panel, !visibleElements.Contains(panel));
        }

        foreach (HideableUIElement button in managedButtons)
        {
            button.SetHidden(!visibleElements.Contains(button));
        }
    }

    private void OnPanelClosed(PanelClosedEvent evt)
    {
        RestoreCurrentState();
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => ApplyCurrentState();
    private void OnSwitchToEdit(SwitchToEditEvent evt) => ApplyCurrentState();
    private void OnEditModeInitialized(EditModeInitializedEvent evt) => ApplyCurrentState();
    private void OnEditModeChange(EditModeChangeEvent evt) => ApplyCurrentState();
    private void OnEnterAnchorAttach(EnterAnchorAttachEvent evt) => ApplyCurrentState();
    private void OnExitAnchorAttach(ExitAnchorAttachEvent evt) => ApplyCurrentState();
    private void OnAnchorSelected(AnchorSelectedEvent evt) => ApplyCurrentState();
    private void OnAnchorDeselected(AnchorDeselectedEvent evt) => ApplyCurrentState();
    private void OnAnchorPositionEditStarted(AnchorPositionEditStartedEvent evt) => ApplyCurrentState();
    private void OnAnchorPositionEditEnded(AnchorPositionEditEndedEvent evt) => ApplyCurrentState();
    
    private void OnEnable()
    {
        eventBus.Subscribe<PanelClosedEvent>(OnPanelClosed);
        eventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Subscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Subscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Subscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Subscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
        eventBus.Subscribe<AnchorSelectedEvent>(OnAnchorSelected);
        eventBus.Subscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
        eventBus.Subscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
        eventBus.Subscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
    }

    private void OnDisable()
    {
        eventBus.Unsubscribe<PanelClosedEvent>(OnPanelClosed);
        eventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        eventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
        eventBus.Unsubscribe<EditModeInitializedEvent>(OnEditModeInitialized);
        eventBus.Unsubscribe<EditModeChangeEvent>(OnEditModeChange);
        eventBus.Unsubscribe<EnterAnchorAttachEvent>(OnEnterAnchorAttach);
        eventBus.Unsubscribe<ExitAnchorAttachEvent>(OnExitAnchorAttach);
        eventBus.Unsubscribe<AnchorSelectedEvent>(OnAnchorSelected);
        eventBus.Unsubscribe<AnchorDeselectedEvent>(OnAnchorDeselected);
        eventBus.Unsubscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
        eventBus.Unsubscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
    }
}