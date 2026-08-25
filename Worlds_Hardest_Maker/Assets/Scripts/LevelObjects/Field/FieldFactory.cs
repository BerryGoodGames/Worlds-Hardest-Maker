using UnityEngine;
using VContainer;
using VContainer.Unity;

public class FieldFactory : ILevelObjectFactory<FieldController>
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
    
    public FieldController Create(ManagerParameters args)
    {
        GameObject prefab = args.FieldMode.Prefab;
        GameObject res = Object.Instantiate(
            prefab, args.Position, Quaternion.Euler(0, 0, args.Rotation),
            args.Sheet == null ? fieldContainer : args.Sheet.AttachmentContainer
        );

        diContainer.InjectGameObject(res);

        FieldController fieldController = res.GetComponent<FieldController>();

        fieldController.Initialize(playerContainer);

        fieldController.FieldMode = args.FieldMode;

        PlaceManager.Instance.AttachToSheet(res, args.Sheet);

        return fieldController;
    }
}