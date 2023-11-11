using UnityEngine;

/// <Summary>
///     Checks inputted key if it's the konami Code
/// </Summary>
public class KonamiManager : MonoBehaviour
{
    public static KonamiManager Instance { get; private set; }

    public bool KonamiActive { get; private set; }

    private int keyIndex;

    // Konami Code: up up down down left right left right BA
    private readonly KeyCode[] konamiKeys =
    {
        KeyCode.UpArrow, KeyCode.UpArrow,
        KeyCode.DownArrow, KeyCode.DownArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.B, KeyCode.A,
    };

    private void Update()
    {
        if (!Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2)) return;

        if (Input.GetKeyDown(konamiKeys[keyIndex]))
        {
            keyIndex++;

            // check if code is finished
            if (keyIndex < konamiKeys.Length) return;

            KonamiActive = !KonamiActive;

            SetKonamiActive(KonamiActive);

            // ReSharper disable once StringLiteralTypo
            print($"Konami {(KonamiActive ? "en" : "dis")}abled");
            keyIndex = 0;
        }
        else keyIndex = 0;
    }

    private static void SetKonamiActive(bool active)
    {
        // toggle key sneezing
        foreach (KeyController key in KeyManager.Instance.Keys) key.KonamiAnimation.enabled = active;

        // toggle shotgun (if player exists)
        PlayerController player = PlayerManager.GetPlayer();
        if (player != null)
        {
            player.Shotgun.gameObject.SetActive((!LevelSessionManager.Instance.IsEdit || EditModeManager.Instance.Playing) && active);
        }

        // mark play try as cheated if enabling
        if (active) PlayManager.Instance.Cheated = true;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}