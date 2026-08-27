using UnityEngine;
using VContainer;
using VContainer.Unity;

public class FieldFactory
{
    private readonly IObjectResolver diContainer;
        
    private Transform fieldContainer;
    private Transform playerContainer;

    public FieldFactory(IObjectResolver diContainer)
    {
        this.diContainer = diContainer;
    }

    public void Initialize(Transform fieldContainer, Transform playerContainer)
    {
        this.fieldContainer = fieldContainer;
        this.playerContainer = playerContainer;
    }
    
    public FieldController Create(Vector2 position, int rotation, ISheet sheet, FieldMode fieldMode)
    {
        GameObject prefab = fieldMode.Prefab;
        AnchorController oldSheet = sheet.ToAnchorOrNull();
        GameObject res = Object.Instantiate(
            prefab, position, Quaternion.Euler(0, 0, rotation),
            oldSheet == null ? fieldContainer : oldSheet.AttachmentContainer
        );

        diContainer.InjectGameObject(res);

        FieldController fieldController = res.GetComponent<FieldController>();

        fieldController.Initialize(playerContainer);

        fieldController.FieldMode = fieldMode;

        PlaceManager.Instance.AttachToSheet(res, sheet);

        return fieldController;
    }
}