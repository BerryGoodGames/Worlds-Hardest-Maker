using JetBrains.Annotations;
using UnityEngine;

public abstract class CopyData
{
    public Data Data;
    public Vector2 RelativePos;
    
    protected CopyData(Data data, Vector2 relativePos)
    {
        Data = data;
        RelativePos = relativePos;
    }
    
    public virtual void Paste(Vector2 pos) => Data.ImportToLevel(pos + RelativePos);
    
    public EditMode GetEditMode() => Data.GetEditMode();
    
    public struct Args
    {
        public Data Data { get; init; }
        public Vector2 RelativePosition { get; init; }
        public AnchorController Sheet { get; init; }
    }
}

public class CopyDataAttachable : CopyData
{
    [CanBeNull] public AnchorController Sheet;
    
    public CopyDataAttachable(Data data, Vector2 relativePos, [CanBeNull] AnchorController sheet) : base(data, relativePos)
    {
        Sheet = sheet;
    }
    
    public CopyDataAttachable(Args args) : base(args.Data, args.RelativePosition)
    {
        Sheet = args.Sheet;
    }
    
    public override void Paste(Vector2 pos) => ((AttachableData)Data).ImportToLevel(Sheet);
}

public class CopyDataNonAttachable : CopyData
{
    public CopyDataNonAttachable(Data data, Vector2 relativePos) : base(data, relativePos) { }
    
    public CopyDataNonAttachable(Args args) : base(args.Data, args.RelativePosition) { }
}