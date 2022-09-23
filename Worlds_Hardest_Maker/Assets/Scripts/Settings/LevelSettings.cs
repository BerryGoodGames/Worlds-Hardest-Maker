using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance { get; private set; }

    #region SETTING VARIABLES
    [HideInInspector] public float drownDuration;
    [HideInInspector] public float waterDamping;
    [HideInInspector] public float iceSlide;
    #endregion

    private void Awake()
    {
        // init singleton
        if (Instance == null) Instance = this;
    }
}
