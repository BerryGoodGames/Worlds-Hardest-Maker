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
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        size = castSize;

        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, 3200);

        clipBoard.Clear();

        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent(out Controller controller))
            {
                IData data = controller.GetData();

                CopyData copyData = new(data, (Vector2)hit.transform.position - lowestPos);
                clipBoard.Add(copyData);
            }
        }
    }

    public static void Paste(Vector2 pos)
    {
        foreach(CopyData copyData in clipBoard)
        {
            copyData.Paste(pos);
        }
    }

    public static IEnumerator StartPaste()
    {
        if (clipBoard.Count == 0) yield break;
        MenuManager.blockMenu = true;
        pasting = true;

        Vector2 mousePos = MouseManager.GetMouseWorldPos();

        int matrixX = (int)(Mathf.Round(mousePos.x) - size.x * 0.5f);
        int matrixY = (int)(Mathf.Round(mousePos.y) - size.x * 0.5f);

        CreatePreview();

        // wait until clicked, cancel if esc is clicked
        while (!Input.GetMouseButton(0))
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                MenuManager.blockMenu = false;
                ClearPreview();
                Instance.previewContainer.position = Vector2.zero;
                pasting = false;
                yield break;
            }

            mousePos = MouseManager.GetMouseWorldPos();

            matrixX = (int)(Mathf.Round(mousePos.x) - size.x * 0.5f);
            matrixY = (int)(Mathf.Round(mousePos.y) - size.x * 0.5f);

            Instance.previewContainer.position = new(matrixX, matrixY);

            yield return null;
        }


        Paste(new(matrixX, matrixY));

        MenuManager.blockMenu = false;

        ClearPreview();
        Instance.previewContainer.position = Vector2.zero;
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
            GameObject preview = Instantiate(GameManager.Instance.FillPreview, Vector2.zero, Quaternion.identity, Instance.previewContainer);

            preview.transform.localPosition = copyData.relativePos;

            PreviewController previewController = preview.GetComponent<PreviewController>();

            previewController.changeSpriteToCurrentEditMode = false;
            previewController.updateEveryFrame = false;
            previewController.showSpriteWhenPasting = true;

            previewController.SetSprite(copyData.GetEditMode());
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
