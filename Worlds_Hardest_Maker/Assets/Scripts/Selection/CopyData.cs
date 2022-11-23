using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyData
{
    public IData data;
    public Vector2 relativePos;

    public CopyData(IData data, Vector2 relativePos)
    {
        this.data = data;
        this.relativePos = relativePos;
    }

    public void Paste(Vector2 pos)
    {
        data.ImportToLevel(pos + relativePos);
    }
}
