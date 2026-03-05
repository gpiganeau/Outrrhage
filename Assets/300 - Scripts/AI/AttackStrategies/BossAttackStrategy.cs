using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static BossAttackStrategySetupData;

public class BossAttackStrategy : AttackStrategy
{
    float timer = 0f;

    MovementContext currentContext;
    private List<BossStrategy> Strategies;

    // -- Strategy -- 
    private BossStrategy currentStrategy;
    private int _currentAttackIndex;
    private int AttackPoolCycle => currentStrategy.AttackOrders.Count;
    private int AttackCount => currentStrategy.AttackSettings.Count;
    float TimeToNextSkill => currentStrategy.TimeToNextSkills;


	public override void Initialize(AttackStrategySetupData setupData, SkillsController controller, MovementController movement)
    {
        var s = setupData as BossAttackStrategySetupData;

        if (s != null)
        {
        
            timer = 0f;
            _controller = controller;
            _movement = movement;
            _currentAttackIndex = 0; // -- Track Attack Pattern

            if (s.Strategies == null) return;

            foreach (var strategie in s.Strategies)
            {
                Strategies.Add(strategie);
            }

            // -- todo : Helper to get appropriate Strategie HP Based
            currentStrategy = Strategies[0];
        }
    }

    public override void Execute()
    {
        timer = 0;
        
        if (Strategies == null) return;

        //var choice = FindClosestAvailableAttack(currentStrategy.AttackSettings, currentContext);

        //-- Our current index
        // -- 
        int nextAttackIndex = currentStrategy.AttackOrders[_currentAttackIndex];
        AttackWrapper nextAttack = currentStrategy.AttackSettings[nextAttackIndex];

        _currentAttackIndex ++;

        bool wait = nextAttack.StopWhenAttacking;
        float waitTime = nextAttack.StopDuration;

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
                _controller.CallSkillStrategy(nextAttackIndex);
            });
                
        } else
        {
                _controller.CallSkillStrategy(nextAttackIndex);
        }
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
