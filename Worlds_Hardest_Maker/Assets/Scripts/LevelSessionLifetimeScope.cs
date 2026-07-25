using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class LevelSessionLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInHierarchy<KonamiManager>().As<IKonamiService>();
        
        // register preview services
        builder.Register<IPreviewVisibilityRulesService, PreviewVisibilityRulesService>(Lifetime.Singleton);
        builder.Register<IPreviewSpriteDataProvider, PreviewSpriteDataProvider>(Lifetime.Singleton);
        builder.Register<IPreviewRotationDataProvider, PreviewRotationDataProvider>(Lifetime.Singleton);
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