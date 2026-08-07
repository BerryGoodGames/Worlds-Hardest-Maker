using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using WorldsHardestMaker.PlayerRecording;

public class LevelSessionLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<KonamiManager>().As<IKonamiService>();
        
        // register preview services
        builder.Register<PreviewSpriteDataProvider>(Lifetime.Singleton);
        builder.Register<PreviewRotationDataProvider>(Lifetime.Singleton);
        builder.Register<PreviewVisibilityRulesService>(Lifetime.Singleton);
        builder.Register<PreviewSpriteAlphaProvider>(Lifetime.Singleton);

        builder.RegisterComponentInHierarchy<MouseManager>().As<IMouseService>();

        builder.RegisterComponentInHierarchy<PlayerRecordingManager>().As<IRecordingService>();

        builder.Register<SelectionState>(Lifetime.Singleton).As<ISelectionState>();
    }
    
    protected override void Awake()
    {
        base.Awake();
        
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject o in roots)
        {
            Container.InjectGameObject(o);
        }
    }
}