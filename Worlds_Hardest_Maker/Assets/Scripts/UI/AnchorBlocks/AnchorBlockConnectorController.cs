using System.Collections;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class AnchorBlockConnectorController : MonoBehaviour
{
    [SerializeField] [PositiveValueOnly] private float detectionHeight;
    [HideInInspector] public MouseOverUIRect MouseOverUIRect;

    private void Start()
    {
        MouseOverUIRect = GetComponent<MouseOverUIRect>();

        MouseOverUIRect.OnHovered = () => AnchorBlockManager.Instance.ExecuteConnectorOnHover = true;
        MouseOverUIRect.OnUnhovered = () => AnchorBlockManager.Instance.ExecuteConnectorOnUnhover = true;
    }

    public void OnHover()
    {
        AnchorBlockManager.Instance.HoveredBlockIndex =
            ReferenceManager.Instance.MainChainController.transform.childCount - 2;

        ReferenceManager.Instance.AnchorBlockPreview.Activate();
    }

    public void OnUnhover()
    {
        AnchorBlockManager.Instance.HoveredBlockIndex = -1;
        ReferenceManager.Instance.AnchorBlockPreview.Deactivate();
    }

    public void UpdateY()
    {
        // move anchor connector
        RectTransform mainChainRectTransform = (RectTransform)ReferenceManager.Instance.MainChainController.transform;
        float anchorY = mainChainRectTransform.localPosition.y;

        foreach (RectTransform anchorBlockInChain in mainChainRectTransform)
        {
            // ignore preview object
            if (anchorBlockInChain.CompareTag("AnchorBlockPreview")) continue;

            float height = anchorBlockInChain.rect.height;

            anchorY -= height;
        }

        RectTransform anchorConnectorRt =
            (RectTransform)ReferenceManager.Instance.AnchorBlockConnectorController.transform;

        anchorConnectorRt.localPosition = new(anchorConnectorRt.localPosition.x, anchorY);
    }

    public void UpdateHeight(Vector2 mouseOffset)
    {
        // update anchor connector y size
        RectTransform rt = (RectTransform)transform;

        float blockHeight = ((RectTransform)AnchorBlockManager.Instance.DraggedBlock.transform).rect.height;
        float offsetY = mouseOffset.y;

        rt.sizeDelta = new(rt.sizeDelta.x, blockHeight * 1.5f - offsetY);
    }

    public void UpdateYAtEndOfFrame() => StartCoroutine(UpdateYCoroutine());

    private IEnumerator UpdateYCoroutine()
    {
        yield return new WaitForEndOfFrame();

        UpdateY();
    }

    private void LateUpdate()
    {
        if (MouseOverUIRect.Over && AnchorBlockManager.Instance.HoveredBlockIndex == -1)
        {
            AnchorBlockManager.Instance.HoveredBlockIndex =
                ReferenceManager.Instance.MainChainController.transform.childCount - 2;
        }
    }
}