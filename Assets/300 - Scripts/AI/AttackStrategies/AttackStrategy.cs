using System.Collections.Generic;
using UnityEngine;

public abstract class AttackStrategy : MonoBehaviour
{
	protected SkillsController _controller;
    protected MovementController _movement;
	public abstract void Initialize(AttackStrategySetupData setupData, SkillsController controller, MovementController movement);
	public abstract void Execute();
	public abstract void Tick(MovementContext context);

    protected (bool, AttackWrapper, int) FindClosestAvailableAttack(List<AttackWrapper> wrappers, MovementContext context)
    {
         // -- Find Closest Action
        float distanceToPlayer = (context.playerPosition - context.currentLocation).magnitude;
        List<float> distances = new();

        if (wrappers == null) return (false, null, 0);

        foreach (var w in wrappers)
        {
            distances.Add(w.MinimumDistanceToRiel);
        }
        
        int index = 0;
        float bestDistance = -1f;
        bool match = false;
        bool wait = false;
        float waitTime = 0;

        for (int i = 0; i < wrappers.Count; i++)
        {
            float minDist = wrappers[i].MinimumDistanceToRiel;
            if (minDist <= distanceToPlayer && minDist > bestDistance)
            {
                bestDistance = minDist;
                index = i;
                match = true;
                waitTime = wrappers[i].StopDuration;
                wait = wrappers[i].StopWhenAttacking;
            }
        }
        if (!match) return (false, null, 0);
        return (true, wrappers[index], index);
    }
}


    [System.Serializable]
    public class AttackWrapper
    {
        public string AttackNameToHelpDesigner = "Help";
        public float MinimumDistanceToRiel;

        [Header("Marque l'arret ?")]
        public bool StopWhenAttacking = false;
        public float StopDuration = 1f;
    }
