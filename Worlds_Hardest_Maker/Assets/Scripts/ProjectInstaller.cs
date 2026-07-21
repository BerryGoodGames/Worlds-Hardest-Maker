using Zenject;

public class ProjectInstaller : MonoInstaller
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    public override void InstallBindings()
    {
        Container.Bind<EventBus>().AsSingle();
        
        Container.Bind<IAudioService>()
            .To<AudioManager>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}