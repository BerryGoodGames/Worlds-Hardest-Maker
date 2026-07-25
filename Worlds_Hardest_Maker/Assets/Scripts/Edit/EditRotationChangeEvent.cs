public class EditRotationChangeEvent
{
    public int Rotation { get; private set; }
    
    public EditRotationChangeEvent(int rotation)
    {
        Rotation = rotation;
    }
}