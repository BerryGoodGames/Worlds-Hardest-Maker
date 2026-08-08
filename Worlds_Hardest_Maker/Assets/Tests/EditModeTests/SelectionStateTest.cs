using NUnit.Framework;
using UnityEngine;

public class SelectionStateTest
{
    private EventBus eventBus;
    private ISelectionState selectionState;
    
    [SetUp]
    public void Setup()
    {
        eventBus = new();
        selectionState = new SelectionState(eventBus);
    }

    [Test]
    public void InitialState_IsNotSelecting()
    {
        Assert.IsFalse(selectionState.IsSelecting);
        Assert.IsNull(selectionState.Start);
        Assert.IsNull(selectionState.End);
    }

    [Test]
    public void SelectionStart_CorrectFields()
    {
        Vector2 beginPosition = new(1, 2);
        
        selectionState.StartSelection(beginPosition);
        
        Assert.IsTrue(selectionState.IsSelecting);
        Assert.AreEqual(beginPosition, selectionState.Start);
        Assert.AreEqual(beginPosition, selectionState.End);
    }

    [Test]
    public void SelectionStart_IsFiringEvent()
    {
        Vector2 beginPosition = new(1, 2);
        SelectionStartedEvent firedEvent = null;
        
        eventBus.Subscribe<SelectionStartedEvent>(e => firedEvent = e);
        
        selectionState.StartSelection(beginPosition);
        
        Assert.IsNotNull(firedEvent);
        Assert.AreEqual(firedEvent.Start, selectionState.Start);
    }

    [Test]
    public void SelectionUpdated_CorrectFields()
    {
        Vector2 start = new(1, 2);
        Vector2 end = new(3, 3);
        
        selectionState.StartSelection(start);
        selectionState.UpdateSelection(start, end);
        
        Assert.AreEqual(start, selectionState.Start);
        Assert.AreEqual(end, selectionState.End);
    }

    [Test]
    public void SelectionUpdated_IsFiringEvent()
    {
        Vector2 start = new(1, 2);
        Vector2 end = new(3, 3);
        SelectionUpdatedEvent firedEvent = null;
        
        eventBus.Subscribe<SelectionUpdatedEvent>(e => firedEvent = e);
        
        selectionState.StartSelection(start);
        selectionState.UpdateSelection(start, end);
        
        Assert.IsNotNull(firedEvent);
        Assert.AreEqual(firedEvent.Start, selectionState.Start);
        Assert.AreEqual(firedEvent.End, selectionState.End);
    }

    [Test]
    public void SelectionEnd_CorrectFields()
    {
        Vector2 start = new(1, 2);
        Vector2 end = new(3, 3);
        
        selectionState.StartSelection(start);
        selectionState.UpdateSelection(start, end);
        selectionState.EndSelection(start, end);
        
        Assert.AreEqual(start, selectionState.Start);
        Assert.AreEqual(end, selectionState.End);
    }

    [Test]
    public void SelectionEnd_IsFiringEvent()
    {
        Vector2 start = new(1, 2);
        Vector2 end = new(3, 3);
        SelectionEndedEvent firedEvent = null;
        
        eventBus.Subscribe<SelectionEndedEvent>(e => firedEvent = e);
        
        selectionState.StartSelection(start);
        selectionState.UpdateSelection(start, end);
        selectionState.EndSelection(start, end);
        
        Assert.IsNotNull(firedEvent);
        Assert.AreEqual(firedEvent.Start, selectionState.Start);
        Assert.AreEqual(firedEvent.End, selectionState.End);
    }
    
    [Test]
    public void SelectionCancel_CorrectFields()
    {
        Vector2 start = new(1, 2);
        Vector2 end = new(3, 3);
        
        selectionState.StartSelection(start);
        selectionState.UpdateSelection(start, end);
        selectionState.EndSelection(start, end);
        selectionState.CancelSelection();
        
        Assert.IsNull(selectionState.Start);
        Assert.IsNull(selectionState.End);
        Assert.IsFalse(selectionState.IsSelecting);
    }

    [Test]
    public void SelectionCancel_IsFiringEvent()
    {
        Vector2 start = new(1, 2);
        Vector2 end = new(3, 3);
        SelectionCancelledEvent firedEvent = null;
        
        eventBus.Subscribe<SelectionCancelledEvent>(e => firedEvent = e);
        
        selectionState.StartSelection(start);
        selectionState.UpdateSelection(start, end);
        selectionState.EndSelection(start, end);
        selectionState.CancelSelection();
        
        Assert.IsNotNull(firedEvent);
    }
}
