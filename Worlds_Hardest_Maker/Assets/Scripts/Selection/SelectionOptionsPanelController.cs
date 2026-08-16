using System;
using UnityEngine;

namespace WorldsHardestMaker.Selection
{
    [Serializable]
    public class SelectionOptionsPanelController : IDisposable
    {
        [SerializeField] private RectTransform panel;
        [SerializeField] private UIAttachToPoint attachment;
    
        private EventBus eventBus;

        public void SetEventBus(EventBus eventBus)
        {
            this.eventBus = eventBus;
        
            eventBus.Subscribe<SelectionStartedEvent>(OnSelectionStarted);
            eventBus.Subscribe<SelectionEndedEvent>(OnSelectionEnded);
            eventBus.Subscribe<SelectionClearedEvent>(OnSelectionCleared);
        }
    
        private void OnSelectionStarted(SelectionStartedEvent evt) => Hide();
        private void OnSelectionEnded(SelectionEndedEvent evt)
        {
            Show();
        
            float width = evt.End.x - evt.Start.x;
            float height = evt.End.y - evt.Start.y;
        
            Vector2 position = new(width < 0 ? evt.End.x - 0.5f : evt.End.x + 0.5f, height < 0 ? evt.End.y - 0.5f : evt.End.y + 0.5f);
        
            attachment.Point = position;
        
            panel.pivot = new(width > 0 ? 0 : 1, height > 0 ? 0 : 1);
        }
        private void OnSelectionCleared(SelectionClearedEvent evt) => Hide();

        private void Show()
        {
            panel.gameObject.SetActive(true);
        }

        private void Hide()
        {
            panel.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            eventBus.Unsubscribe<SelectionStartedEvent>(OnSelectionStarted);
            eventBus.Unsubscribe<SelectionEndedEvent>(OnSelectionEnded);
            eventBus.Unsubscribe<SelectionClearedEvent>(OnSelectionCleared);
        }
    }
}