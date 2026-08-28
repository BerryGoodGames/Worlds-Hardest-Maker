using UnityEngine;
using VContainer;
using VContainer.Unity;

public class FieldFactory
{
    private readonly IObjectResolver diContainer;
    private readonly IAttachmentService attachmentService;
        
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
        GameObject res = Object.Instantiate(
            prefab, position, Quaternion.Euler(0, 0, rotation),
            sheet is AnchorSheet anchorSheet1 ? anchorSheet1.Container : fieldContainer
        );

        diContainer.InjectGameObject(res);

        FieldController fieldController = res.GetComponent<FieldController>();

        fieldController.Initialize(playerContainer);

        fieldController.FieldMode = fieldMode;

        if(sheet is AnchorSheet anchorSheet2) attachmentService.Attach(fieldController, anchorSheet2.Anchor);

        return fieldController;
    }
}