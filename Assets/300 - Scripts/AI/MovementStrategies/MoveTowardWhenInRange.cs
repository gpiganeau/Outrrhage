using UnityEngine;
	
public class MoveTowardWhenInRange: MovementStrategy
{
    float DistanceToRiel;

    public override void Initialize(MovementStrategySetupData setupData)
    {
        MoveTowardWhenInRangeSetupData data = setupData as MoveTowardWhenInRangeSetupData;
        if (data != null)
        {
            DistanceToRiel = data.DistanceToRiel;
        }
    }

    public override Vector3 GetMovementDirection(MovementContext context)
    {
        float distanceToPlayer = (context.playerPosition - context.currentLocation).magnitude;
        Vector3 directionToPlayer = (context.playerPosition - context.currentLocation).normalized;

        if (distanceToPlayer <= DistanceToRiel)
        {
            // Move towards the player
            return directionToPlayer;
        }
        else
        {
            // Stay in place
            return Vector3.zero;
        }
    }

    public override Vector3 GetFacingDirection(MovementContext context)
    {
        return (context.playerPosition - context.currentLocation).normalized;
    }
}
