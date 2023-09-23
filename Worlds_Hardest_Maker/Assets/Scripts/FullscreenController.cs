using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    private void Start() =>
        // Set the display mode to fullscreen 
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,
            FullScreenMode.FullScreenWindow);
}