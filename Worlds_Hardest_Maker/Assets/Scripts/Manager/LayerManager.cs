using System;
using System.Reflection;
using JetBrains.Annotations;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance { get; private set; }
    
    public LayerVariables Layers;
    public SortingLayerVariables SortingLayers;
    
    [field: SerializeField] [field: MyBox.ReadOnly] public string[] AllSortingLayerNames { get; private set; }
    [field: SerializeField] [field: MyBox.ReadOnly] public int[] AllSortingLayerIDs { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    #if UNITY_EDITOR
    
    [ButtonMethod]
    [UsedImplicitly]
    public void UpdateSortingLayerLists()
    {
        AllSortingLayerNames = GetSortingLayerNames();
        AllSortingLayerIDs = GetSortingLayerUniqueIDs();
        print("Successfully updated sorting layer lists");
    }
    
    private static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, Array.Empty<object>());
    }
    
    private static int[] GetSortingLayerUniqueIDs()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty(
            "sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic
        );
        
        return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
    }
    
    #endif
}

[Serializable]
public class LayerVariables
{
    [InitializationField] [Required] public LayerMask Default;
    [InitializationField] [Required] public LayerMask TransparentFX;
    [InitializationField] [Required] public LayerMask IgnoreRaycast;
    [InitializationField] [Required] public LayerMask Water;
    [InitializationField] [Required] public LayerMask UI;
    [InitializationField] [Required] public LayerMask Entity;
    [InitializationField] [Required] public LayerMask Player;
    [InitializationField] [Required] public LayerMask Void;
    [InitializationField] [Required] public LayerMask Field;
    [InitializationField] [Required] public LayerMask Background;
    [InitializationField] [Required] public LayerMask DoNotCollide;
    
    public LayerMask LevelObjectMask => Entity | Player | Void | Field;
}

[Serializable]
public class SortingLayerVariables
{
    [InitializationField] [Required] public string Background;
    [InitializationField] [Required] public string AnchorBelow;
    [InitializationField] [Required] public string Field;
    [InitializationField] [Required] public string Coin;
    [InitializationField] [Required] public string Key;
    [InitializationField] [Required] public string Player;
    [InitializationField] [Required] public string Default;
    [InitializationField] [Required] public string Outline;
    [InitializationField] [Required] public string Anchor;
    [InitializationField] [Required] public string Ball;
    [InitializationField] [Required] public string AnchorAbove;
    [InitializationField] [Required] public string PlayerPlayMode;
    [InitializationField] [Required] public string FillPreview;
    [InitializationField] [Required] public string PlacementPreview;
    [InitializationField] [Required] public string Line;
}