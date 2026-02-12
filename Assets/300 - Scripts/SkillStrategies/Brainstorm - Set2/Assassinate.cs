using DG.Tweening;
using UnityEngine;

class Assassinate : SkillStrategy
{
    int amountHealed = 0;
    float bloodToHealRatio = 0.5f;

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
            startingPosition = movementController.transform.position + firingDirection.normalized * 1.5f,
            BloodStackingAmount = _storedSkillData.BloodStackingAmount,
        };

        Projectile projectile = SpawnProjectile(projectileData, 0);

        DOVirtual.DelayedCall(_storedSkillData.ProjectileLifetime, PostLifetimeEffects);
        PutInCooldown();
    
        return true;
    }

    public override int CustomDamageCalculation(DamageController target, int baseDamage, Projectile projectile)
    {
        if (target == null)
        {
            Logger.LogError(Logger.LogCategory.Combat, "DamageController target is null in CustomDamageCalculation.");
            return baseDamage;
        }

        if (target.GetComponent<BloodStack>() == null)
        {
            Logger.LogError(Logger.LogCategory.Combat, "Target does not have a BloodStack component for CustomDamageCalculation.");
            return baseDamage;
        }

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
