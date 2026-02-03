using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SlashStrategy: SkillStrategy
{
    private bool hasHitATarget = false;
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        hasHitATarget = false;

        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 2f * movementController.GetFacingDirection(),
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage,
            Lifetime = _storedSkillData.ProjectileLifetime,
            Team = team,
        };

        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");
        SpawnProjectile(projectileData);

        DOVirtual.DelayedCall(_storedSkillData.ProjectileLifetime, PostLifetimeEffects);
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "SlashAttack");
        });
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
        {
            movementController.SetImmobilized(false, "SlashAttack");
        });
        PutInCooldown();
        return true;
    }

    protected override void OnProjectileHit(Projectile projectile)
    {
        hasHitATarget = true;
    }

    private void PostLifetimeEffects()
    {
        if (!hasHitATarget)
        {
            //Spawn bloodlet
        }
    }
}
