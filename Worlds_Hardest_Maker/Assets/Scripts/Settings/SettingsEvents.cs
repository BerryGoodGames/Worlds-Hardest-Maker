public class SetToolbarSizeEvent
{
    public float Size { get; private set; }
    
    public SetToolbarSizeEvent(float size) => Size = size;
}

public class SetInfobarSizeEvent
{
    public float Size { get; private set; }
    
    public SetInfobarSizeEvent(float size) => Size = size;
}

public class SetOneColorSafeFieldsEvent
{
    public bool IsOneColor { get; private set; }
    
    public SetOneColorSafeFieldsEvent(bool isOneColor) => IsOneColor = isOneColor;
}

public class SetShowRoomGridEvent
{
    public bool ShowRoomGrid { get; private set; }
    
    public SetShowRoomGridEvent(bool showRoomGrid) => ShowRoomGrid = showRoomGrid;
}