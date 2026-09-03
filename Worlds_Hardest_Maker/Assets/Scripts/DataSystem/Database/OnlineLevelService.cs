using Client = Supabase.Client;

public class OnlineLevelService
{
    private readonly Client supabase;
    private readonly SaveSystem saveSystem;

    public OnlineLevelService(Client supabase, SaveSystem saveSystem)
    {
        this.supabase = supabase;
        this.saveSystem = saveSystem;
    }
    
}