using UnityEngine;
using System.Collections;


public interface IMovementStrategyInterface
{
	public void Initialize(MovementStrategySetupData setupData);
    public Vector3 GetMovementDirection(MovementContext context);
}
