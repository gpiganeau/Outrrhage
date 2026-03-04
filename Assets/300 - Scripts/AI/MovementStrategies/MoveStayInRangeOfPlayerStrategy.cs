using System;
using System.Collections.Generic;
using UnityEngine;
	
public class MoveStayInRangeOfPlayerStrategy: MovementStrategy
{
    float minDistanceToPlayer;
    float maxDistanceToPlayer;

    [Serializable]
    public class Pallier
    {
        public int SwitchAtHp = 1;
        public Vector2 MinMaxDistanceToPlayer;
    }

    List<Pallier> palliers = new();


    public override void Initialize(MovementStrategySetupData setupData)
    {
        MoveStayInRangeOfPlayerSetupData data = setupData as MoveStayInRangeOfPlayerSetupData;
        if (data != null)
        {
            minDistanceToPlayer = data.minDistanceFromPlayer;
            maxDistanceToPlayer = data.maxDistanceFromPlayer;

            palliers.Clear();
            foreach (var p in data.palliers)
            {
                palliers.Add(p);
            }
        }
    }

    private void RecomputeDistances(MovementContext c)
    {

        if (palliers == null) return;
        
        foreach (var p in palliers)
        {
            if (p.SwitchAtHp == c.currentHealth)
            {
                minDistanceToPlayer = p.MinMaxDistanceToPlayer.x;
                maxDistanceToPlayer = p.MinMaxDistanceToPlayer.y;
                break;
            }
        }
    }

    public override Vector3 GetMovementDirection(MovementContext context)
    {
        float distanceToPlayer = (context.playerPosition - context.currentLocation).magnitude;
        Vector3 directionToPlayer = (context.playerPosition - context.currentLocation).normalized;


        RecomputeDistances(context);


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

    public override Vector3 GetFacingDirection(MovementContext context)
    {
        // Always face the player
        return (context.playerPosition - context.currentLocation).normalized;
    }
}
