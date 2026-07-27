using MyBox;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [Separator] [SerializeField] [MustBeAssigned] private AudioManager audioManagerPrefab;
    [SerializeField] [MustBeAssigned] private ToastManager toastManagerPrefab;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<EventBus>(Lifetime.Singleton);
        
        builder.RegisterComponentInNewPrefab(audioManagerPrefab, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<IAudioService>();
        
        builder.RegisterComponentInNewPrefab(toastManagerPrefab, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<IToastService>();
        
        builder.RegisterBuildCallback(container =>
        {
            container.Resolve<IAudioService>();
            container.Resolve<IToastService>();
        });
    }
}