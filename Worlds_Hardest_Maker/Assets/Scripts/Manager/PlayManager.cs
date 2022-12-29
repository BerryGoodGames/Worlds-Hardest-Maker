using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance { get; set; }

    private bool cheated;
    public bool Cheated
    {
        get => cheated;
        set
        {
            cheated = value;
            TextManager.Instance.timer.color = cheated ? TextManager.Instance.cheatedTimerColor : Color.black;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}
