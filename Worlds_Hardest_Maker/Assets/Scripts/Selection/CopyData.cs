using UnityEngine;

public class CopyData
{
    public Data Data;
    public Vector2 RelativePos;

    public CopyData(Data data, Vector2 relativePos)
    {
        Data = data;
        RelativePos = relativePos;
    }

    public void Paste(Vector2 pos) => Data.ImportToLevel(pos + RelativePos);

    public EditMode GetEditMode() => Data.GetEditMode();
}