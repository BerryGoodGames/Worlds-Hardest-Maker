using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance { get; private set; }

    [Header("Objects")]
    public GameObject Manager;
    public GameObject Canvas;
    public GameObject TooltipCanvas;
    public GameObject Menu;
    public GameObject PlayButton;
    public GameObject PlacementPreview;
    public GameObject LevelSettingsPanel;
    public GameObject BallWindows;
    [Space]
    [Header("Containers")]
    public Transform ToolbarContainer;
    public Transform SliderContainer;
    public Transform NameTagContainer;
    public Transform DrawContainer;
    public Transform SelectionOutlineContainer;
    public Transform FillPreviewContainer;
    public Transform PlayerContainer;
    public Transform AnchorContainer;
    public Transform BallDefaultContainer;
    public Transform BallCircleContainer;
    public Transform CoinContainer;
    public Transform KeyContainer;
    public Transform FieldContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}
