using UnityEngine;
using System.Collections;


public abstract class MovementStrategy : MonoBehaviour
{
	public abstract void Initialize(MovementStrategySetupData setupData);
    public abstract Vector3 GetMovementDirection(MovementContext context);
}
