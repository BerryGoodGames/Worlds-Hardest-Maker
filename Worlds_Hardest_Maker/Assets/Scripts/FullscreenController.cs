using System;
using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    [SerializeField] private bool preventInWebGL = true;
    
    private void Start()
    {
        if (preventInWebGL && Application.platform == RuntimePlatform.WebGLPlayer) return;
        
        // Set the display mode to fullscreen 
        Screen.SetResolution(
            Screen.currentResolution.width, Screen.currentResolution.height,
            FullScreenMode.FullScreenWindow
        );
    }
}