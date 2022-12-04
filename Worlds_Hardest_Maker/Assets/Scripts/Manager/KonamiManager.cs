using System;
using UnityEngine;
///<Summary>
///Checks inputted key if it's the konami Code
///</Summary>
public class KonamiManager : MonoBehaviour
{
    private int keyIndex = 0;
    public static bool KonamiActive { get; set; } = false;

    // Konami Code: ???????BA
    private readonly KeyCode[] konamiKeys = {
        KeyCode.UpArrow, KeyCode.UpArrow, 
        KeyCode.DownArrow, KeyCode.DownArrow, 
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
        {
            if (Input.GetKeyDown(konamiKeys[keyIndex]))
            {
                keyIndex++;
                // check if code is finished
                if (keyIndex >= konamiKeys.Length)
                {
                    KonamiActive = !KonamiActive;

                    KeyManager.SetKonamiMode(KonamiActive);

                    print($"Konami {(KonamiActive ? "en" : "dis")}abled");
                    keyIndex = 0;
                }
            }
            else keyIndex = 0;
        }
    }
}