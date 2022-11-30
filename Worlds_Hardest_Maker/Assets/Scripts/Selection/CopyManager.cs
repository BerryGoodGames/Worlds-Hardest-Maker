using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    private static List<CopyData> clipBoard = new();
    private static CopyManager Instance { get; set; }

    private static Vector2 size = Vector2.zero;

    public static bool pasting = false;

    [SerializeField] private Transform previewContainer;

    public static void Copy(Vector2 lowestPos, Vector2 highestPos)
    {
        // get position and size on where to get the objects
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        size = castSize; // save size

        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, 3200); // get objects

        clipBoard.Clear();

        foreach(Collider2D hit in hits)
        {
            // try to get controllers and save the object in clipboard
            if(hit.TryGetComponent(out Controller controller))
            {
                IData data = controller.GetData();

                // special case for ball circles
                Vector2 pos = controller.GetType() == typeof(BallCircleController)? ((BallCircleController)controller).origin.position : (Vector2)hit.transform.position;

                CopyData copyData = new(data, pos - lowestPos);
                clipBoard.Add(copyData);
            }
        }
    }

    public static void Paste(Vector2 pos)
    {
        // just load clipboard to pos
        foreach(CopyData copyData in clipBoard)
        {
            copyData.Paste(pos);
        }
    }

    public static IEnumerator StartPaste()
    {
        // check if there smth. in clipboard
        if (clipBoard.Count == 0) yield break;
        // block menu from beeing openend and some other stuff
        MenuManager.blockMenu = true;
        pasting = true;

        // get position where to paste
        Vector2 mousePos = MouseManager.GetMouseWorldPos();

        int matrixX = (int)(Mathf.Round(mousePos.x) - size.x * 0.5f);
        int matrixY = (int)(Mathf.Round(mousePos.y) - size.x * 0.5f);

        CreatePreview();

        // wait until clicked, cancel if esc is clicked
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if(Input.GetKey(KeyCode.Escape) || GameManager.Instance.Selecting || GameManager.Instance.Playing)
            {
                MenuManager.blockMenu = false;
                ClearPreview();
                Instance.previewContainer.position = Vector2.zero;
                pasting = false;
                yield break;
            }

            // update position on where to paste
            mousePos = MouseManager.GetMouseWorldPos();

            matrixX = (int)(Mathf.Round(mousePos.x) - size.x * 0.5f);
            matrixY = (int)(Mathf.Round(mousePos.y) - size.x * 0.5f);

            Instance.previewContainer.position = new(matrixX, matrixY);

            yield return null;
        }

        // paste
        Paste(new(matrixX, matrixY));

        // remove some blocks etc.
        MenuManager.blockMenu = false;

        ClearPreview();
        Instance.previewContainer.position = Vector2.zero;
        // make sure that the player cant place directly after pasting
        while (!Input.GetMouseButtonUp(0))
            yield return null;

        pasting = false;
        yield break;
    }

    private static void CreatePreview()
    {
        ClearPreview();

        foreach(CopyData copyData in clipBoard)
        {
            Quaternion rotation = copyData.data.GetType() == typeof(FieldData) ? Quaternion.Euler(0, 0, ((FieldData)copyData.data).rotation) : Quaternion.identity;
            GameObject preview = Instantiate(GameManager.Instance.FillPreview, Vector2.zero, rotation, Instance.previewContainer);

            preview.transform.localPosition = copyData.relativePos;

            PreviewController previewController = preview.GetComponent<PreviewController>();

            // set some settings in preview
            previewController.changeSpriteToCurrentEditMode = false;
            previewController.updateEveryFrame = false;
            previewController.showSpriteWhenPasting = true;

            // set spire of preview
            previewController.SetSprite(copyData.GetEditMode(), false);
        }
    }

    private static void ClearPreview()
    {
        foreach(Transform child in Instance.previewContainer)
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
