using UnityEngine;


public abstract class MovementStrategy : MonoBehaviour
{
	public abstract void Initialize(MovementStrategySetupData setupData);
    public abstract Vector3 GetMovementDirection(MovementContext context);
    public abstract Vector3 GetFacingDirection(MovementContext context);
}
