using UnityEngine;
using UnityEngine.Serialization;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance { get; private set; }

    [FormerlySerializedAs("lineMaterial")] [Header("Materials")]
    public Material LineMaterial;

    [FormerlySerializedAs("noFriction")] public PhysicsMaterial2D NoFriction;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}