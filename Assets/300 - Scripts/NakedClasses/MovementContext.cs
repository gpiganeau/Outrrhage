using UnityEngine;
using System.Collections;

public class MovementContext
{
	public Vector3 currentLocation;
	public Vector3 playerPosition;

	public int currentHealth;
	
	public MovementContext(Vector3 currentLocation, Vector3 playerPosition, int currentHealth)
	{
		this.currentLocation = currentLocation;
		this.playerPosition = playerPosition;
		this.currentHealth = currentHealth;
    }
}
