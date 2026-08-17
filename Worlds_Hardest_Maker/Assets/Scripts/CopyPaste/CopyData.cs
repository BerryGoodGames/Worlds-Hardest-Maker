using UnityEngine;

namespace WorldsHardestMaker.CopyPaste
{
    public struct CopyData
    {
        public readonly Data Data;
        public readonly Vector2 RelativePos;
    
        public CopyData(Data data, Vector2 relativePos)
        {
            Data = data;
            RelativePos = relativePos;
        }
    }
}