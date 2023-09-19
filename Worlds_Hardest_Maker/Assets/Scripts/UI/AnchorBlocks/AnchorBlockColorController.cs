using MyBox;
using UnityEditor;
using UnityEngine;

public abstract class AnchorBlockColorController : MonoBehaviour
{
    protected const float Darkening = 0.3f;
    // protected const float DarkerDarkening = 0.75f;

    [SerializeField] protected Color Color;

    [ButtonMethod]
    public abstract void UpdateColor();

    public void SetColor(Color c) => Color = c;

    protected static Color GetDarkenedColor(Color color, float darkening)
    {
        float percent = 1 - darkening;
        Color darker = new(color.r * percent, color.g * percent, color.b * percent);
        return darker;
    }

    protected static Color KeepA(Color @new, Color assign) => new(@new.r, @new.g, @new.b, assign.a);

    [ButtonMethod]
    private void UpdateColorButton()
    {
        UpdateColor();

#if UNITY_EDITOR
        EditorApplication.QueuePlayerLoopUpdate();
#endif
    }
}