using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CopyManager : MonoBehaviour
{
    private static List<CopyData> copyDataList = new();
    private static CopyManager Instance { get; set; }

    private static Vector2 size = Vector2.zero;

    [SerializeField] private Transform previewContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public static void Copy(Vector2 lowestPos, Vector2 highestPos)
    {
        Vector2 castPos = Vector2.Lerp(lowestPos, highestPos, 0.5f);
        Vector2 castSize = highestPos - lowestPos;

        size = castSize;

        Collider2D[] hits = Physics2D.OverlapBoxAll(castPos, castSize, 0, 3200);

        copyDataList.Clear();

        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent(out Controller controller))
            {
                IData data = controller.GetData();

                CopyData copyData = new(data, (Vector2)hit.transform.position - lowestPos);
                copyDataList.Add(copyData);
            }
        }
    }

    public static void Paste(Vector2 pos)
    {
        foreach(CopyData copyData in copyDataList)
        {
            copyData.Paste(pos);
        }
    }

    public static IEnumerator StartPaste()
    {
        MenuManager.blockMenu = true;

        Vector2 mousePos = MouseManager.GetMouseWorldPos();

        int matrixX = (int)Mathf.Round(mousePos.x);
        int matrixY = (int)Mathf.Round(mousePos.y);

        CreatePreview();

        // wait until clicked, cancel if esc is clicked
        while (!Input.GetMouseButtonDown(0))
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                MenuManager.blockMenu = false;
                ClearPreview();
                Instance.previewContainer.position = Vector2.zero;
                yield break;
            }

            mousePos = MouseManager.GetMouseWorldPos();

            matrixX = (int)Mathf.Round(mousePos.x);
            matrixY = (int)Mathf.Round(mousePos.y);

            Instance.previewContainer.position = new(matrixX, matrixY);

            yield return null;
        }

        

        Paste(new(matrixX, matrixY));
        MenuManager.blockMenu = false;
        ClearPreview();
        Instance.previewContainer.position = Vector2.zero;
        yield break;
    }

    private static void CreatePreview()
    {
        ClearPreview();

        foreach(CopyData copyData in copyDataList)
        {
            GameObject preview = Instantiate(GameManager.Instance.FillPreview, Vector2.zero, Quaternion.identity, Instance.previewContainer);

            preview.transform.localPosition = copyData.relativePos;

            PreviewController previewController = preview.GetComponent<PreviewController>();

            previewController.changeSpriteToCurrentEditMode = false;
            previewController.updateEveryFrame = false;

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
}
