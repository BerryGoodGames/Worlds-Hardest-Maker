using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance { get; private set; } // singleton

    [Header("Materials")]
    public Material LineMaterial;
    public PhysicsMaterial2D NoFriction;

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}
