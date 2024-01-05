using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnchorBlockSource : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject anchorBlockPrefab;
    [SerializeField] private bool active = true;

    public void CreateNew()
    {
        Vector2 position = transform.position;

        GameObject anchorBlock = Instantiate(
            anchorBlockPrefab,
            position,
            Quaternion.identity,
            ReferenceManager.Instance.AnchorBlockChainContainer
        );

        // activate restriction
        UIRestrictInRectTransform restrict = anchorBlock.GetComponent<UIRestrictInRectTransform>();
        restrict.RectTransform = ReferenceManager.Instance.AnchorBlockChainContainer;

        // rebuild
        AnchorBlockRebuilder rebuilder = anchorBlock.GetComponent<AnchorBlockRebuilder>();
        rebuilder.RebuildLayout();

        // transition to dragging
        anchorBlock.GetComponent<AnchorBlockDragDrop>().BeginDrag();
        
        // play sfx
        AudioManager.Instance.Play("AnchorBlockPickUp");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active && eventData.button == PointerEventData.InputButton.Left) CreateNew();
    }

    [ButtonMethod]
    public void SetNonInteractable()
    {
        AnchorBlockPositionInputController[] positionInputs =
            GetComponentsInChildren<AnchorBlockPositionInputController>();

        foreach (AnchorBlockPositionInputController positionInput in positionInputs)
        {
            TMP_InputField[] inputs = positionInput.GetComponentsInChildren<TMP_InputField>();
            Button[] buttons = positionInput.GetComponentsInChildren<Button>();

            foreach (TMP_InputField inputField in inputs)
            {
                inputField.interactable = false;
                inputField.transition = Selectable.Transition.None;
            }

            foreach (Button button in buttons)
            {
                button.interactable = false;
                button.transition = Selectable.Transition.None;
            }
        }
    }
}