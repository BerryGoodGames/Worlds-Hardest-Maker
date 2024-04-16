using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    private void Move()
    {
        if (Won) return;
        bool ice = IsOnIce();

        Vector2 totalMovement = Vector2.zero;
        // movement (if player is yours in multiplayer mode)
        if (LevelSessionEditManager.Instance.Playing)
        {
            if (ice) IcePhysics();
            else AddMovement(ref totalMovement);
        }

        AddConveyorMovement(ref totalMovement);

        if (totalMovement != Vector2.zero) Rb.MovePosition(Rb.position + totalMovement);
    }

    private void ApplyForcesFromFloor()
    {
        List<FieldController> floors = GetFullyOnFields();
        
        if (floors.Count == 0) return;
        
        List<Vector2> forces = new();
        Vector2 finalForce = new();
        int count = 0;
        foreach (FieldController field in floors)
        {
            Vector2 force = field.DeltaPosition;
            
            if (forces.Contains(force)) continue;
            
            finalForce += force;
            forces.Add(force);
            count++;
        }

        Rb.position += finalForce / count;
    }

    private void UpdateWaterState()
    {
        // check water and update drown level
        bool onWaterNow = IsOnWater();
        if (!onWater && onWaterNow)
            // frame player enters water
            AudioManager.Instance.Play("WaterEnter");

        onWater = onWaterNow;

        if (onWater && !InDeathAnim && !Won)
        {
            currentDrownDuration += Time.fixedDeltaTime;

            if (currentDrownDuration >= LevelSettings.Instance.DrownDuration) DieNormal("DeathDrown");
        }
        else if (!InDeathAnim && !onWater) { currentDrownDuration = 0; }

        if (LevelSettings.Instance.DrownDuration == 0) return;

        float drown = currentDrownDuration / LevelSettings.Instance.DrownDuration;
        waterLevel.localScale = new(waterLevel.localScale.x, drown);
    }

    private void IcePhysics()
    {
        // transfer velocity to ice when entering
        if (Rb.velocity == Vector2.zero) Rb.velocity = GetPhysicsSpeed() * movementInput;

        Rb.drag = LevelSettings.Instance.IceFriction;

        // acceleration on ice
        // convert to units / second
        float force = LevelSettings.Instance.IceMaxSpeed;
        Rb.AddForce(force * LevelSettings.Instance.IceFriction * movementInput, ForceMode2D.Force);
    }

    private void AddMovement(ref Vector2 totalMovement)
    {
        Rb.velocity = Vector2.zero;

        // snappy movement (when not on ice)
        if (!movementInput.Equals(Vector2.zero))
            totalMovement += GetPhysicsSpeed() * Time.fixedDeltaTime * new Vector2(
                Mathf.Clamp(movementInput.x + extraMovementInput.x, -1, 1),
                Mathf.Clamp(movementInput.y + extraMovementInput.y, -1, 1)
            );

        extraMovementInput = Vector2.zero;
    }

    private void AddConveyorMovement(ref Vector2 totalMovement)
    {
        ConveyorController conveyor = GetCurrentConveyor();
        if (conveyor == null) return;

        Quaternion forceRotation = Quaternion.Euler(0, 0, conveyor.Rotation);

        Vector2 conveyorVector = conveyor.Speed * Time.fixedDeltaTime * (forceRotation * Vector2.up);

        totalMovement += conveyorVector;
    }

    private float GetPhysicsSpeed() => onWater ? LevelSettings.Instance.WaterDampingFactor * Speed : Speed;

    private void CornerPush(Collision2D collider)
    {
        Vector2 roundedPos = new(Mathf.Round(Rb.position.x), Mathf.Round(Rb.position.y));

        const float err = 0.00001f;

        // do wall corner pushy thingy
        if (!collider.transform.tag.IsSolidFieldTag() ||
            (!collider.transform.position.x.EqualsFloat(roundedPos.x + movementInput.x) &&
             !collider.transform.position.y.EqualsFloat(roundedPos.y + movementInput.y))) return;

        CornerPushHorizontal(collider, roundedPos, err);
        CornerPushVertical(collider, roundedPos, err);
    }

    private void CornerPushVertical(Collision2D collider, Vector2 roundedPos, float err)
    {
        // early-out if it should not push 
        if (movementInput.y == 0 || roundedPos.x.EqualsFloat(Mathf.Round(collider.transform.position.x)) ||
            !(Mathf.Abs(Rb.position.x) % 1 > (1 - transform.lossyScale.x) * 0.5f + err) ||
            !(Mathf.Abs(Rb.position.x) % 1 < 1 - ((1 - transform.lossyScale.x) * 0.5f + err))) return;

        // check if player counteracts
        if (movementInput.x != 0) return;

        // calculate new position 
        extraMovementInput = new(Mathf.Round(Rb.position.x) > Rb.position.x ? 1 : -1, movementInput.y);
    }

    private void CornerPushHorizontal(Collision2D collider, Vector2 roundedPos, float err)
    {
        // early-out if it should not push 
        if (movementInput.x == 0
            || roundedPos.y.EqualsFloat(Mathf.Round(collider.transform.position.y))
            || !(Mathf.Abs(Rb.position.y) % 1 > (1 - transform.lossyScale.y) * 0.5f + err)
            || !(Mathf.Abs(Rb.position.y) % 1 < 1 - ((1 - transform.lossyScale.y) * 0.5f + err))) return;


        // check if player counteracts
        if (movementInput.y != 0) return;

        // calculate new position
        extraMovementInput = new(movementInput.x, Mathf.Round(Rb.position.y) > Rb.position.y ? 1 : -1);
    }
}
