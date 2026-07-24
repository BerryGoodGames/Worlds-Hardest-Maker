using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [SerializeField] [MustBeAssigned] private AudioManager audioManagerPrefab;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<EventBus>(Lifetime.Singleton);
        
        builder.RegisterComponentInNewPrefab(audioManagerPrefab, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<IAudioService>();
        
        builder.RegisterBuildCallback(container =>
        {
            container.Resolve<IAudioService>();
        });
    }
}