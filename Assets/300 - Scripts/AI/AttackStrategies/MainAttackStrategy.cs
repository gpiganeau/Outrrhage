using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainAttackStrategy : AttackStrategy
{
    float TimeToNextSkill;
    float timer = 0f;

    private List<AttackWrapper> AttackSettings = new();

    MovementContext currentContext;


	public override void Initialize(AttackStrategySetupData setupData, SkillsController controller, MovementController movement)
    {
        var s = setupData as MainAttackStrategySetupData;

        if (s != null)
        {
            
            timer = 0f;
            _controller = controller;
            _movement = movement;

            TimeToNextSkill = s.TimeToNextSkill;

            if (s.AttackSettings == null) return;

            foreach (var wrapper in s.AttackSettings)
            {
                AttackSettings.Add(wrapper);
            }
        }
    }





    public override void Execute()
    {
        timer = 0;

        // -- Find Closest Action
        float distanceToPlayer = (currentContext.playerPosition - currentContext.currentLocation).magnitude;

        List<float> distances = new();

        if (AttackSettings == null) return;

        foreach (var w in AttackSettings)
        {
            distances.Add(w.MinimumDistanceToRiel);
        }
        
        int index = 0;
        float bestDistance = -1f;
        bool match = false;
        bool wait = false;
        float waitTime = 0;

        for (int i = 0; i < AttackSettings.Count; i++)
        {
            float minDist = AttackSettings[i].MinimumDistanceToRiel;
            if (minDist <= distanceToPlayer && minDist > bestDistance)
            {
                bestDistance = minDist;
                index = i;
                match = true;
                waitTime = AttackSettings[i].StopDuration;
                wait = AttackSettings[i].StopWhenAttacking;
            }
        }


        if (!match) return;

        // -- Stop Time
        if (wait)
        {

            // -- Grab & Stop Movement controller ?
            _movement.SetImmobilized(true, "AttackStrategy");

            timer -= waitTime;
            DOVirtual.DelayedCall(waitTime, () =>
            {
                // Release Movement Controller
                _movement.SetImmobilized(false, "AttackStrategy");
                _controller.CallSkillStrategy(index);
            });
                
        } else
        {
                _controller.CallSkillStrategy(index);
        }
        
    }

    private void LegacyRandomBehavior()
    {
        timer += Time.deltaTime;

        if (timer > Random.Range(0.8f, 1.2f))
        {
            timer = 0;
            _controller.CallRandomSkill();
        }
    }


    public override void Tick(MovementContext context)
    {
        //LegacyRandomBehavior();
        // ------------------------------------------------ //

        // -- One Timer
        timer += Time.deltaTime;
        currentContext = context;

        if (timer > TimeToNextSkill)
        {
            Execute();
        }

        // ------------------------------------------------- //

    }
}
