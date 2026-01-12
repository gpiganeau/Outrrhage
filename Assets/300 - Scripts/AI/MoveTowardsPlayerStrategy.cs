using System.Collections;
using UnityEngine;


public class MoveTowardsPlayerStrategy : MonoBehaviour, IMovementStrategyInterface
{
    public void Initialize(MovementStrategySetupData setupData)
    {
        // No initialization needed for this simple strategy
    }

    public Vector3 GetMovementDirection(MovementContext context)
    {
        return (context.playerPosition - context.currentLocation).normalized;
    }
}
