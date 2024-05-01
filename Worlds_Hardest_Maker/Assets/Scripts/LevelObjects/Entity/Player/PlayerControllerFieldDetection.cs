using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public partial class PlayerController
{
    public bool IsOnSafeField()
    {
        if (!LevelSettings.Instance.PlayerInvincibility) return false;
        
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is safe
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType.IsSafeForPlayer) return true;
        }
        
        return false;
    }
    
    public bool IsOnField(FieldMode mode)
    {
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is type
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == mode) return true;
        }
        
        return false;
    }
    
    public bool IsOnFieldInSheet(FieldMode mode, [CanBeNull] AnchorController sheet)
    {
        foreach (FieldController field in CurrentFields)
        {
            // check if current field is type
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == mode && IManager.IsInSheet(field, sheet)) return true;
        }
        
        return false;
    }
    
    public List<FieldController> GetFullyOnFields()
    {
        // finds every field the player is at least half way on
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.08f);
        List<FieldController> res = new();
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out FieldController f)) res.Add(f);
        }
        
        return res;
    }
    
    private bool IsStandingOnPlatformMode(FieldMode mode) => CurrentPlatforms.FindIndex(field => field.FieldMode == mode) != -1;
    
    public bool IsFullyOnField(FieldMode mode)
    {
        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == mode) return true;
        }
        
        return false;
    }
    public bool IsOnMode(FieldMode mode) => (!IsStandingOnPlatform && IsFullyOnField(mode)) || IsStandingOnPlatformMode(mode);

    
    public ConveyorController GetCurrentConveyor()
    {
        if (!IsFullyOnField(EditModeManager.Conveyor)) return null;
        
        List<FieldController> fullyOnFields = GetFullyOnFields();
        foreach (FieldController field in fullyOnFields)
        {
            FieldMode currentFieldType = field.FieldMode;
            if (currentFieldType == EditModeManager.Conveyor) return field.GetComponent<ConveyorController>();
        }
        
        return null;
    }
    
    public FieldController GetCurrentField() => FieldManager.Instance.Get(Vector2Int.RoundToInt(transform.position));
}