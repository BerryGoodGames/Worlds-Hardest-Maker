using MyBox;
using NaughtyAttributes;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance { get; private set; }
    
    [Header("Materials")] [InitializationField] [Required] public Material LineMaterial;
    
    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}