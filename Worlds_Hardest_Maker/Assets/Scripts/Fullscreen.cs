using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fullscreen : MonoBehaviour
{
    [SerializeField] private bool fullscreen;
    private void Start()
    {
        Screen.fullScreen = fullscreen;
    }
}
