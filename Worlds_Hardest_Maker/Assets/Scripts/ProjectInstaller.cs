using MyBox;
using UnityEngine;
using Zenject;

/// <summary>
/// Installs bindings in Zenject that last for the entire game, e.g. audio manager.
/// Should be assigned to the ProjectContext prefab in Assets/Resources.
/// </summary>
public class ProjectInstaller : MonoInstaller
{
    [SerializeField] [MustBeAssigned] private AudioManager audioManagerPrefab;
    
    public override void InstallBindings()
    {
        Container.Bind<EventBus>().AsSingle().NonLazy();
        
        Container.Bind<IAudioService>()
            .To<AudioManager>()
            .FromComponentInNewPrefab(audioManagerPrefab)
            .AsSingle();
    }
}