public class EditModeChangeEvent
{
    public EditMode NewEditMode { get; private set; }
    
    public EditModeChangeEvent(EditMode newEditMode)
    {
        NewEditMode = newEditMode;
    }
}