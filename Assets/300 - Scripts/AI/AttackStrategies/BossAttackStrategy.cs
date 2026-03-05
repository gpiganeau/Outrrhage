using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static BossAttackStrategySetupData;

public class BossAttackStrategy : AttackStrategy
{
    float TimeToNextSkill;
    float timer = 0f;

    MovementContext currentContext;

    private List<BossStrategy> Strategies;

	public override void Initialize(AttackStrategySetupData setupData, SkillsController controller, MovementController movement)
    {
        var s = setupData as BossAttackStrategySetupData;

        if (s != null)
        {
            
            TimeToNextSkill = 0;
            timer = 0f;
            _controller = controller;
            _movement = movement;

            if (s.Strategies == null) return;

            foreach (var strategie in s.Strategies)
            {
                Strategies.Add(strategie);
            }
        }
    }

    public override void Execute()
    {
        timer = 0;
        
        if (Strategies == null) return;

        float distanceToPlayer = (currentContext.playerPosition - currentContext.currentLocation).magnitude;



        
    }

    public override void Tick(MovementContext context)
    {
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
