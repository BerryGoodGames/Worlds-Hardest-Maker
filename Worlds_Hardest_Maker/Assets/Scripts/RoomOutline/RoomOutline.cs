using MyBox;
using UnityEngine;

public class RoomOutline : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private SpriteRenderer top, left, bottom, right;
    
    public void SetDimensions(int width, int height)
    {
        top.transform.localScale = new(width, top.transform.localScale.y);
        left.transform.localScale = new(left.transform.localScale.x, height);
        bottom.transform.localScale = new(width, bottom.transform.localScale.y);
        right.transform.localScale = new(right.transform.localScale.x, height);
        
        top.transform.localPosition = new(0, -(float)height / 2);
        left.transform.localPosition = new(-(float)width / 2, 0);
        bottom.transform.localPosition = new(0, (float)height / 2);
        right.transform.localPosition = new((float)width / 2, 0);
    }
}