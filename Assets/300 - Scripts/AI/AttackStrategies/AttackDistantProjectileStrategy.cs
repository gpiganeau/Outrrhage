using UnityEngine;
using System.Collections;

public class AttackDistantProjectileStrategy : AttackStrategy
{
    float fireRate = 1f;
    float timeToFire = 0f;

	public override void Initialize(AttackStrategySetupData setupData, SkillsController controller)
    {
        fireRate = Random.Range(0.8f, 1.2f);
        timeToFire = 0f;
        _controller = controller;
    }

    public override void Execute()
    {
        timeToFire = 0;
        _controller.CallRandomSkill();
    }

    public override void Tick()
    {
        timeToFire += Time.deltaTime;

        if (timeToFire > fireRate)
        {
            Execute();
        }
    }
}
