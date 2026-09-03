using MyBox;
using Supabase;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ProjectLifetimeScope : LifetimeScope
{
    [Separator] [SerializeField] [MustBeAssigned] private AudioManager audioManagerPrefab;
    [SerializeField] [MustBeAssigned] private ToastCanvas toastCanvasPrefab;
    [SerializeField] [MustBeAssigned] private ToastManager toastManagerPrefab;
    [SerializeField] [MustBeAssigned] private SupabaseConfig supabaseConfig;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<EventBus>(Lifetime.Singleton);
        
        builder.RegisterComponentInNewPrefab(audioManagerPrefab, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<IAudioService>();
        
        builder.RegisterComponentInNewPrefab(toastCanvasPrefab, Lifetime.Singleton)
            .DontDestroyOnLoad();
        
        builder.RegisterComponentInNewPrefab(toastManagerPrefab, Lifetime.Singleton)
            .DontDestroyOnLoad()
            .As<IToastService>();
        
        builder.RegisterBuildCallback(container =>
        {
            container.Resolve<IAudioService>();
            container.Resolve<ToastCanvas>();
            container.Resolve<IToastService>();
        });
        
        builder.Register<Client>(_ =>
        {
            SupabaseOptions options = new()
            {
                AutoConnectRealtime = false,
            };
            return new(supabaseConfig.ProjectUrl, supabaseConfig.AnonKey, options);
        }, Lifetime.Singleton);

        builder.Register<OnlineLevelService>(Lifetime.Singleton).AsSelf();

        builder.RegisterBuildCallback(async container =>
        {
            Client supabase = container.Resolve<Client>();
            await supabase.InitializeAsync();
        });
    }
}