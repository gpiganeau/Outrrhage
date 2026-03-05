using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MeleeDashStrategy: SkillStrategy
{
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
        movementController.SetImmobilized(true, "MeleeDashAttack");
        parentController.SetSkillsDisabled(true, "MeleeDashAttack");
        UseAimAssist(ref firingDirection, _storedSkillData.AimAssistRatio, team);

        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 2f * firingDirection,
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[0],
            Lifetime = _storedSkillData.movementDuration, // Dure exactement le temps du dash
            BloodStackingAmount = _storedSkillData.BloodStackingAmount,
            Team = team,
        };

        Projectile spawnedProjectile = SpawnProjectile(projectileData, 0);
        if (spawnedProjectile != null)
            StartCoroutine(TrackCasterDuringDash(spawnedProjectile, movementController, firingDirection, _storedSkillData.movementDuration));

        movementController.Dash(
            movementController.GetFacingDirection(),
            _storedSkillData.movementDistance,
            _storedSkillData.movementDuration,
            _storedSkillData.ignoreCollisions,
            onComplete: () => 
            {
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

    /// <summary>
    /// Pendant toute la durée du dash, repositionne le projectile devant le caster chaque frame.
    /// </summary>
    private IEnumerator TrackCasterDuringDash(Projectile projectile, MovementController movementController, Vector3 initialDirection, float dashDuration)
    {
        float elapsed = 0f;

        while (elapsed < dashDuration && projectile != null && projectile.gameObject.activeInHierarchy)
        {
            Vector3 offset = initialDirection * 2f;
            projectile.transform.position = movementController.transform.position + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ← Kill le projectile proprement à la fin du dash
        //if (projectile != null && projectile.gameObject.activeInHierarchy)
            //projectile.ForceExpire(); // -- DO NOT ABUSE THIS, IT S A BUG FIX
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
            var pos = cachedController.transform.position;
            pos -= cachedController.transform.forward * 2.5f;
            pos = pos.WithY(1);
            Instantiate(bloopDropPrefab, pos, Quaternion.identity);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
