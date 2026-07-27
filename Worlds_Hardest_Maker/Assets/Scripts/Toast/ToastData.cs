using UnityEngine;

public struct ToastData
{
    public string Message { get; init; }
    public float Duration { get; init; }
    public Sprite Sprite { get; init; }
    public Color Color { get; init; }
    
    public ToastData(string message, float duration, Sprite sprite, Color color)
    {
        Message = message;
        Duration = duration;
        Sprite = sprite;
        Color = color;
    }
}