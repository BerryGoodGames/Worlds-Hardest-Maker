using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager Instance { get; private set; }

    public bool Multiplayer { get; set; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
