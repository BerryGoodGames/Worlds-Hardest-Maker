using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    public static CopyManager Instance { get; set; }

    private readonly List<CopyData> clipBoard = new();

    public bool Pasting;

    [SerializeField] private Transform previewContainer;

    public void Copy(Vector2 lowestPos, Vector2 highestPos)
    {
        clipBoard.Clear();

        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        // get position and size on where to get the objects
        Vector2 selectionCenter = (lowestPos + highestPos) * .5f;
        Vector2 selectionSize = highestPos - lowestPos + Vector2.one * .5f;

        Collider2D[] hits = Physics2D.OverlapBoxAll(selectionCenter, selectionSize, 0, 3200); // get objects

        List<Vector2> points = HitsToPoints(hits);

        if (points.Count == 0) return;

        (Vector2 lowest, Vector2 highest) = SelectionManager.GetBoundsMatrix(points);

        // center and size of actual controllers user selected
        Vector2 castCenter = (.5f * (lowest + highest)).Floor();

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            // try to get controllers and save the object in clipboard
            if (!hit.TryGetComponent(out EntityController controller)) continue;

            Data data = controller.GetData();

            Vector2 pos = controller.Position;

            CopyData copyData = new(data, pos - castCenter);
            clipBoard.Add(copyData);
        }
    }

    private static List<Vector2> HitsToPoints(Collider2D[] hits)
    {
        List<Vector2> points = new();
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            // try to get controllers and save the object in clipboard
            if (!hit.TryGetComponent(out EntityController controller)) continue;

            points.Add(controller.Position);
        }

        return points;
    }

    public IEnumerator PasteCoroutine()
    {
        // check if there smth. in clipboard
        if (clipBoard.Count == 0) yield break;

        StartPaste();

        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || SelectionManager.Instance.Selecting || EditModeManager.Instance.Playing)
            {
                CancelPaste();
                yield break;
            }

            yield return null;
        }

        Paste();

        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }

        Pasting = false;
    }

    private void StartPaste()
    {
        // // actions the frame the user starts pasting
        Pasting = true;

        // block menu from being opened and some other stuff
        MenuManager.Instance.BlockMenu = true;

        CreatePreview();

        // hide panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(true);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(true);
        ReferenceManager.Instance.PlayButtonTween.TweenToY(-125, false);
    }

    private void CancelPaste()
    {
        // // actions the frame the user cancels pasting via esc, playing or selecting sth
        MenuManager.Instance.BlockMenu = false;

        ClearPreview();

        Instance.previewContainer.position = Vector2.zero;
        Pasting = false;

        // show panels (if in edit mode)
        ReferenceManager.Instance.ToolbarTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.PlayButtonTween.SetPlay(EditModeManager.Instance.Playing);
    }

    private void Paste()
    {
        // // actions to actually paste
        // get position where to paste
        Vector2 mousePos = MouseManager.Instance.MouseWorldPosMatrix;

        // paste
        LoadClipboard(mousePos);

        // remove some blocks etc.
        MenuManager.Instance.BlockMenu = false;

        ClearPreview();
        Instance.previewContainer.position = Vector2.zero;

        // show bars
        ReferenceManager.Instance.ToolbarTween.SetPlay(false);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(false);
        ReferenceManager.Instance.PlayButtonTween.SetPlay(false);
    }

    public void LoadClipboard(Vector2 pos)
    {
        // just load clipboard to pos
        foreach (CopyData copyData in clipBoard)
        {
            copyData.Paste(pos);
        }
    }

    private void CreatePreview()
    {
        ClearPreview();

        foreach (CopyData copyData in clipBoard)
        {
            Quaternion rotation = copyData.Data.GetType() == typeof(FieldData)
                ? Quaternion.Euler(0, 0, ((FieldData)copyData.Data).Rotation)
                : Quaternion.identity;
            GameObject preview = Instantiate(PrefabManager.Instance.FillPreview, Vector2.zero, rotation,
                Instance.previewContainer);

            preview.transform.localPosition = copyData.RelativePos;

            PreviewController previewController = preview.GetComponent<PreviewController>();

            // set some settings in preview
            previewController.CheckUpdateEveryFrame = false;
            previewController.ShowSpriteWhenPasting = true;
            previewController.RotateToEditRotation = false;

            // set spire of preview
            previewController.SetSprite(copyData.GetEditMode());
        }
    }

    private static void ClearPreview()
    {
        foreach (Transform child in Instance.previewContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
}