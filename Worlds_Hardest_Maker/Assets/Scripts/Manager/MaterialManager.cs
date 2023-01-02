using UnityEngine;
using UnityEngine.Serialization;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance { get; private set; }

    [FormerlySerializedAs("LineMaterial")] [Header("Materials")]
    public Material lineMaterial;

    [FormerlySerializedAs("NoFriction")] public PhysicsMaterial2D noFriction;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}