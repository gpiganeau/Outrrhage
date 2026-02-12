using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class Assassinate : SkillStrategy
{
    int amountHealed = 0;
    float bloodToHealRatio = 0.5f;

    public override void Initialize(SkillsController parent, SkillData skillData)
    {
        base.Initialize(parent, skillData);
    }

    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team))
            return false;

        Vector3 firingDirection = movementController.GetFacingDirection();
        UseAimAssist(ref firingDirection, _storedSkillData.AimAssistRatio, team);

        ProjectileData projectileData = new ProjectileData
        {
            attackStrategy = this,
            Damage = _storedSkillData.ProjectileDamage[0],
            useCustomDamageCalculation = true,
            Team = team,
            Lifetime = _storedSkillData.ProjectileLifetime,
            origin = movementController.transform.position,
            startingPosition = movementController.transform.position,
        };

        Projectile projectile = SpawnProjectile(projectileData, 0);

        DOVirtual.DelayedCall(_storedSkillData.ProjectileLifetime, PostLifetimeEffects);
        PutInCooldown();
    
        return true;
    }

    public override void Release(MovementController movementController, Team team)
    {
        base.Release(movementController, team);
    }

    public override int CustomDamageCalculation(DamageController target, int baseDamage, Projectile projectile)
    {
        int bloodAmount = target.GetComponent<BloodStack>().GetStackedValue();
        if (bloodAmount > 0) 
        {
            int damage = baseDamage + 1 + bloodAmount;
            if (target.CurrentHealth < damage)
            {
                amountHealed = Mathf.CeilToInt(bloodAmount * bloodToHealRatio);
            }
            return damage; 
        }
        else
        {
            return baseDamage;
        }
    }

    private void PostLifetimeEffects()
    {
        if (amountHealed > 0)
        {
            parentController.GetComponent<DamageController>().Heal(amountHealed);
            amountHealed = 0;
        }
    }
}
