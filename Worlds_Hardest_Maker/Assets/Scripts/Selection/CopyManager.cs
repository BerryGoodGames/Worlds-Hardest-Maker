using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    private static List<CopyData> clipBoard = new();
    private static CopyManager Instance { get; set; }

    private static Vector2 size = Vector2.zero;

    public static bool pasting;

    [SerializeField] private Transform previewContainer;

    public static void Copy(Vector2 lowestPos, Vector2 highestPos)
    {
        // get position and size on where to get the objects
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        size = castSize; // save size

        Collider2D[] hits = new Collider2D[Mathf.CeilToInt(castSize.x) * Mathf.CeilToInt(castSize.y)];
        _ = Physics2D.OverlapBoxNonAlloc(castPos, castSize, 0, hits, 3200); // get objects

        clipBoard.Clear();

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            // try to get controllers and save the object in clipboard
            if (!hit.TryGetComponent(out Controller controller)) continue;

            Data data = controller.GetData();

            // special case for ball circles
            Vector2 pos = controller.GetType() == typeof(BallCircleController)
                ? ((BallCircleController)controller).origin.position
                : (Vector2)hit.transform.position;

            CopyData copyData = new(data, pos - lowestPos);
            clipBoard.Add(copyData);
        }
    }

    public static void LoadClipboard(Vector2 pos)
    {
        // just load clipboard to pos
        foreach (CopyData copyData in clipBoard)
        {
            copyData.Paste(pos);
        }
    }

    public static IEnumerator PasteCoroutine()
    {
        // check if there smth. in clipboard
        if (clipBoard.Count == 0) yield break;

        StartPaste();

        // wait until clicked, cancel if esc is clicked
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || SelectionManager.Instance.Selecting || EditModeManager.Instance.Playing)
            {
                CancelPaste();
                yield break;
            }

            // update position on where to paste
            Vector2 mousePos = MouseManager.GetMouseWorldPos();

            int matrixX = (int)(Mathf.Round(mousePos.x) - size.x * 0.5f);
            int matrixY = (int)(Mathf.Round(mousePos.y) - size.x * 0.5f);

            // TODO: smooth animation
            Instance.previewContainer.position = new(matrixX, matrixY);

            yield return null;
        }

        Paste();

        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0))
            yield return null;

        pasting = false;
    }

    private static void StartPaste()
    {
        // // actions the frame the user starts pasting
        pasting = true;

        // block menu from being opened and some other stuff
        MenuManager.Instance.blockMenu = true;
        
        CreatePreview();

        // hide toolbar
        ReferenceManager.Instance.toolbarTween.SetPlay(true);
    }

    private static void CancelPaste()
    {
        // // actions the frame the user cancels pasting via esc, playing or selecting sth
        MenuManager.Instance.blockMenu = false;

        ClearPreview();

        Instance.previewContainer.position = Vector2.zero;
        pasting = false;

        // show toolbar (if in edit mode)
        ReferenceManager.Instance.toolbarTween.SetPlay(EditModeManager.Instance.Playing);
    }

    private static void Paste()
    {
        // // actions to actually paste
        // get position where to paste
        Vector2 mousePos = MouseManager.GetMouseWorldPos();

        int matrixX = (int)(Mathf.Round(mousePos.x) - size.x * 0.5f);
        int matrixY = (int)(Mathf.Round(mousePos.y) - size.x * 0.5f);

        // paste
        LoadClipboard(new(matrixX, matrixY));

        // remove some blocks etc.
        MenuManager.Instance.blockMenu = false;

        ClearPreview();
        Instance.previewContainer.position = Vector2.zero;

        // show toolbar
        ReferenceManager.Instance.toolbarTween.SetPlay(false);
    }

    private static void CreatePreview()
    {
        ClearPreview();

        foreach (CopyData copyData in clipBoard)
        {
            Quaternion rotation = copyData.data.GetType() == typeof(FieldData)
                ? Quaternion.Euler(0, 0, ((FieldData)copyData.data).rotation)
                : Quaternion.identity;
            GameObject preview = Instantiate(PrefabManager.Instance.fillPreview, Vector2.zero, rotation,
                Instance.previewContainer);

            preview.transform.localPosition = copyData.relativePos;

            PreviewController previewController = preview.GetComponent<PreviewController>();

            // set some settings in preview
            previewController.changeSpriteToCurrentEditMode = false;
            previewController.updateEveryFrame = false;
            previewController.showSpriteWhenPasting = true;
            previewController.rotateToRotation = false;

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