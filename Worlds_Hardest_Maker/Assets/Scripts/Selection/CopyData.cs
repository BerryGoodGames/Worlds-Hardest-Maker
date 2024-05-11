using JetBrains.Annotations;
using UnityEngine;

public abstract class CopyData
{
    public Data Data;
    public Vector2 RelativePos;
    
    protected CopyData(Data data, Vector2 relativePos, [CanBeNull] AnchorController sheet)
    {
        Data = data;
        RelativePos = relativePos;
    }
    
    public virtual void Paste(Vector2 pos) => Data.ImportToLevel(pos + RelativePos);
    
    public EditMode GetEditMode() => Data.GetEditMode();
}

public class CopyDataAttachable : CopyData
{
    [CanBeNull] public AnchorController Sheet;
    
    public CopyDataAttachable(Data data, Vector2 relativePos, [CanBeNull] AnchorController sheet) : base(data, relativePos, sheet)
    {
        Sheet = sheet;
    }
    
    public override void Paste(Vector2 pos) => ((AttachableData)Data).ImportToLevel(Sheet);
}

public class CopyDataNonAttachable : CopyData
{
    public CopyDataNonAttachable(Data data, Vector2 relativePos, [CanBeNull] AnchorController sheet) : base(data, relativePos, sheet) { }
    
    
}