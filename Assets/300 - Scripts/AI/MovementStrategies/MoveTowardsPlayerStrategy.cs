using System.Collections;
using UnityEngine;


public class MoveTowardsPlayerStrategy : MovementStrategy
{
    public override void Initialize(MovementStrategySetupData setupData)
    {
        // No initialization needed for this simple strategy
    }

    public override Vector3 GetMovementDirection(MovementContext context)
    {
        return (context.playerPosition - context.currentLocation).normalized;
    }

    public override Vector3 GetFacingDirection(MovementContext context)
    {
        // Always face the player
        return (context.playerPosition - context.currentLocation).normalized;
    }
}
