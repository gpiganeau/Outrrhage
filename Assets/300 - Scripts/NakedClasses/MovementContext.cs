using UnityEngine;
using System.Collections;

public class MovementContext
{
	public Vector3 currentLocation;
	public Vector3 playerPosition;
	
	public MovementContext(Vector3 currentLocation, Vector3 playerPosition)
	{
		this.currentLocation = currentLocation;
		this.playerPosition = playerPosition;
    }
}
