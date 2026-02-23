using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MeleeDashStrategy: SkillStrategy
{
    public override void Initialize(SkillsController parent, SkillData skillData)
    {
        base.Initialize(parent, skillData);
    }

    private bool hasHitATarget = false;
    public BloodDrop bloopDropPrefab;

    MovementController cachedController;

    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        hasHitATarget = false;
        cachedController = movementController;
        Vector3 firingDirection = movementController.GetFacingDirection();

        movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);
        movementController.Dash(movementController.GetFacingDirection(), _storedSkillData.movementDistance, _storedSkillData.movementDuration, _storedSkillData.ignoreCollisions);

        movementController.SetImmobilized(true, "MeleeDashAttack");
        parentController.SetSkillsDisabled(true, "MeleeDashAttack");

        UseAimAssist(ref firingDirection, _storedSkillData.AimAssistRatio, team);

        Vector3 dashTarget = movementController.transform.position + firingDirection * 1.5f;
        //movementController.transform.DOMove(dashTarget, 0.15f).SetEase(Ease.OutQuad);
        movementController.transform.DOMove(dashTarget, 0.1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            ProjectileData projectileData = new ProjectileData()
            {
                startingPosition = movementController.transform.position + 2f * firingDirection,
                origin = movementController.transform.position,
                Damage = _storedSkillData.ProjectileDamage[0],
                Lifetime = _storedSkillData.ProjectileLifetime,
                BloodStackingAmount = _storedSkillData.BloodStackingAmount,
                Team = team,
            };

            SpawnProjectile(projectileData, 0);
			movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);


            DOVirtual.DelayedCall(_storedSkillData.ProjectileLifetime, PostLifetimeEffects);
            DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
            {
                parentController.SetSkillsDisabled(false, "MeleeDashAttack");
            });
            DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
            {
                movementController.SetImmobilized(false, "MeleeDashAttack");
            });
            PutInCooldown();
        });



        return true;

    }

    protected override void OnProjectileHit(Projectile projectile, DamageController damageController)
    {
        base.OnProjectileHit(projectile, damageController);
        StackBlood(projectile, damageController);
        hasHitATarget = true;
    }

    private void PostLifetimeEffects()
    {
        if (!hasHitATarget && _storedSkillData.DropBloodOnFailedSkill)
        {
            //Spawn bloodlet
            var pos = cachedController.transform.position;
            pos -= cachedController.transform.forward * 2.5f;
            pos = pos.WithY(1);
            Instantiate(bloopDropPrefab, pos, Quaternion.identity);
        }
    }
}
