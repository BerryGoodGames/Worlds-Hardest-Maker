using Zenject;

public class ProjectInstaller : MonoInstaller
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    public override void InstallBindings()
    {
    }
}