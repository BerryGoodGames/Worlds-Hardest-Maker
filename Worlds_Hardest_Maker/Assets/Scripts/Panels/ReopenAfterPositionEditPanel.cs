using System;
using UnityEngine;
using VContainer;

namespace WorldsHardestMaker.Panels
{
    [RequireComponent(typeof(PanelController))]
    public class ReopenAfterPositionEditPanel : MonoBehaviour
    {
        private PanelController panelController;

        private bool wasOpenBeforeEdit;

        private EventBus eventBus;
        [Inject] private IPanelService panelService;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            this.eventBus = eventBus;
            
            eventBus.Subscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
            eventBus.Subscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
        }

        private void OnDestroy()
        {
            eventBus.Unsubscribe<AnchorPositionEditStartedEvent>(OnAnchorPositionEditStarted);
            eventBus.Unsubscribe<AnchorPositionEditEndedEvent>(OnAnchorPositionEditEnded);
        }

        private void Awake()
        {
            panelController = GetComponent<PanelController>();
        }

        private void OnAnchorPositionEditStarted(AnchorPositionEditStartedEvent evt)
        {
            wasOpenBeforeEdit = panelController.IsOpen;
        }
        
        private void OnAnchorPositionEditEnded(AnchorPositionEditEndedEvent evt)
        {
            if (wasOpenBeforeEdit)
            {
                panelService.SetPanelOpen(panelController, true);
            }
        }
    }
}