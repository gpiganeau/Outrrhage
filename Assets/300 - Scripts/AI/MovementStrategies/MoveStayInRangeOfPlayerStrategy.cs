using UnityEngine;
using System.Collections;
	
public class MoveStayInRangeOfPlayerStrategy: MovementStrategy
{
    float minDistanceToPlayer;
    float maxDistanceToPlayer;

    public override void Initialize(MovementStrategySetupData setupData)
    {
        MoveStayInRangeOfPlayerSetupData data = setupData as MoveStayInRangeOfPlayerSetupData;
        if (data != null)
        {
            minDistanceToPlayer = data.minDistanceFromPlayer;
            maxDistanceToPlayer = data.maxDistanceFromPlayer;
        }
    }

    public override Vector3 GetMovementDirection(MovementContext context)
    {
        float distanceToPlayer = (context.playerPosition - context.currentLocation).magnitude;
        Vector3 directionToPlayer = (context.playerPosition - context.currentLocation).normalized;
        if (distanceToPlayer < minDistanceToPlayer)
        {
            // Move away from the player
            return -directionToPlayer;
        }
        else if (distanceToPlayer > maxDistanceToPlayer)
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
}
