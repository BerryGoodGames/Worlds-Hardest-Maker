using UnityEngine;

public class CopyData
{
    public Data data;
    public Vector2 relativePos;

    public CopyData(Data data, Vector2 relativePos)
    {
        this.data = data;
        this.relativePos = relativePos;
    }

    public void Paste(Vector2 pos)
    {
        data.ImportToLevel(pos + relativePos);
    }

    public EditMode GetEditMode()
    {
        return data.GetEditMode();
    }
}