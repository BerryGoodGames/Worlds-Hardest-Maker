using UnityEngine;

public class SetAngularSpeedBlock : AnchorBlock
{
    public enum Unit
    {
        ITERATIONS,
        DEGREES,
        TIME
    }

    public const Type BlockType = Type.SET_ANGULAR_SPEED;
    public override Type ImplementedBlockType => BlockType;

    private readonly float speed;
    private readonly Unit unit;

    public SetAngularSpeedBlock(AnchorController anchor, float speed, Unit unit) : base(anchor)
    {
        this.speed = unit switch
        {
            Unit.ITERATIONS => speed * 360,
            _ => speed
        };

        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.ApplyAngularSpeed = unit != Unit.TIME;
        Anchor.AngularSpeed = speed;
        Anchor.FinishCurrentExecution();
    }

    public float GetSpeed(Unit unit)
    {
        return unit switch
        {
            Unit.ITERATIONS => speed * 360,
            _ => speed
        };
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.SetAngularSpeedBlockPrefab, parent);

        // set values in object
        SetAngularSpeedBlockController controller = block.GetComponent<SetAngularSpeedBlockController>();
        controller.SpeedInput.text = GetSpeed(Unit.DEGREES).ToString();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData()
    {
        return new SetAngularSpeedBlockData(speed, unit);
    }
}