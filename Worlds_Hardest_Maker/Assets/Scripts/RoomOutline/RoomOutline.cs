using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class RoomOutline : MonoBehaviour
{
    [SerializeField] [InitializationField] [Required] private Transform top, left, bottom, right;
    
    public void SetDimensions(int width, int height)
    {
        top.localScale = new(width, top.localScale.y);
        left.localScale = new(left.localScale.x, height);
        bottom.localScale = new(width, bottom.localScale.y);
        right.localScale = new(right.localScale.x, height);
        
        top.localPosition = new(0, -(float)height / 2);
        left.localPosition = new(-(float)width / 2, 0);
        bottom.localPosition = new(0, (float)height / 2);
        right.localPosition = new((float)width / 2, 0);
    }
}