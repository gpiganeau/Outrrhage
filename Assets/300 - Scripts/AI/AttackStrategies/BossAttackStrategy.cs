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
    private int AttackCount => AttackWrappers.Count;
    float TimeToNextSkill => currentStrategy.TimeToNextSkills;
    List<AttackWrapper> AttackWrappers;

    int DeathAtHp;

	public override void Initialize(AttackStrategySetupData setupData, SkillsController controller, MovementController movement)
    {
        var s = setupData as BossAttackStrategySetupData;

        if (s != null)
        {
            if (s.Strategies == null) return;
        
            timer = 0f;
            _controller = controller;
            _movement = movement;
            _currentAttackIndex = 0; // -- Track Attack Pattern
            Strategies = new List<BossStrategy>();
            AttackWrappers = s.AttackWrappers;

            foreach (var strategie in s.Strategies)
            {
                Strategies.Add(strategie);
            }

            currentStrategy = Strategies[0];

            DeathAtHp = s.BattleEndAtHealth;

            var damageController = movement.GetComponent<DamageController>();
            if (damageController != null)
                damageController.OnDamaged.AddListener(OnHealthChanged);
        }
    }

    public override void Execute()
    {
        timer = 0;
        
        if (Strategies == null) return;

        // -- Select Correct Wrapper & Loop
        int orderIndex = _currentAttackIndex % AttackPoolCycle; 
        int nextAttackIndex = currentStrategy.AttackOrders[orderIndex];
        _currentAttackIndex++;
        AttackWrapper nextAttack = AttackWrappers[nextAttackIndex];

        // -- Apply Attack
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

    private void OnHealthChanged(int currentHp, int maxHp)
    {

        if (currentHp <= DeathAtHp)
        {
            GetComponent<AIActorComponent>().ForceKill();
            return;
        }


        BossStrategy best = Strategies[0];
        foreach (var strat in Strategies)
        {
            if (currentHp <= strat.HealthThreshold && strat.HealthThreshold > best.HealthThreshold)
                best = strat;
        }

        if (best == currentStrategy) return;

        // Switch
        currentStrategy = best;
        _currentAttackIndex = 0;
        timer = 0f;
        Logger.Combat($"[Boss] Switch strategy → {currentStrategy.strategyName}");
    }


        void OnDestroy()
        {
            var damageController = _movement?.GetComponent<DamageController>();
            if (damageController != null)
                damageController.OnDamaged.RemoveListener(OnHealthChanged);
        }
}
