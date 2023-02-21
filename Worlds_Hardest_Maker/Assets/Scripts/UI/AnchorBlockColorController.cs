using UnityEngine;

public abstract class AnchorBlockColorController : MonoBehaviour
{
    [SerializeField] protected Color color;
    [Range(0, 1)][SerializeField] protected float darkening;

    public abstract void UpdateColor();
    
    public void SetColor(Color c) => color = c;
    public void SetDarkening(float d) => darkening = d;

    public static Color GetDarkColor(Color color, float darkening)
    {
        float percent = 1 - darkening;
        Color darker = new(color.r * percent, color.g * percent, color.b * percent);
        return darker;
    }
    public static Color KeepA(Color new_, Color assign) => new(new_.r, new_.g, new_.b, assign.a);
}
