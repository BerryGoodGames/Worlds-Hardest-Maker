using UnityEngine;

public class Fullscreen : MonoBehaviour
{
    [SerializeField] private bool fullscreen;

    private void Start() => Screen.fullScreen = fullscreen;
}