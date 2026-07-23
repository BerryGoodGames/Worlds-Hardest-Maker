using Zenject;

public class LevelSessionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IKonamiService>()
            .To<KonamiManager>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}