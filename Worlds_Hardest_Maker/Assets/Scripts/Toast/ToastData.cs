public struct ToastData
{
    public string Message { get; init; }
    public float Duration { get; init; }
    public ToastType Type { get; init; }
    
    public ToastData(string message, float duration, ToastType type)
    {
        Message = message;
        Duration = duration;
        Type = type;
    }
}