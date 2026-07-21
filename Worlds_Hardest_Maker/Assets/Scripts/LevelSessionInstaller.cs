using Zenject;

public class LevelSessionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<EventBus>().AsSingle();
        
        Container.Bind<IKonamiService>()
            .To<KonamiManager>()
            .FromComponentsInHierarchy()
            .AsSingle();
    }
}