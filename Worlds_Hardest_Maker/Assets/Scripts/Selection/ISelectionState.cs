using UnityEngine;

public interface ISelectionState
{
    bool IsSelecting { get; }
    Vector2? Start { get; }
    Vector2? End { get; }

    void StartSelection(Vector2 start);
    void UpdateSelection(Vector2 start, Vector2 end);
    void EndSelection(Vector2 start, Vector2 end);
    void CancelSelection();
}