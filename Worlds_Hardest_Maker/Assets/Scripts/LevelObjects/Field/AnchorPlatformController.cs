using MyBox;
using UnityEngine;

public class AnchorPlatformController : FieldController
{
    [SerializeField] [InitializationField] private Color color1;
    [SerializeField] [InitializationField] private Color color2;
    
    protected override void Start()
    {
        base.Start();
        
        Vector2 matrixPosition = ((Vector2)transform.position).Floor();
        
        Color color = (matrixPosition.x + matrixPosition.y) % 2 == 0 ? color1 : color2;
        
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
    }
}