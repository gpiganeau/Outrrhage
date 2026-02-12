using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class BloodSpikes : SkillStrategy
{
    int currentSpike = 0;
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;
        Vector3 firingDirection = movementController.GetFacingDirection();
        UseAimAssist(ref firingDirection, _storedSkillData.AimAssistRatio, team);

        switch (currentSpike) { 
            case 0: SpikeSkillShot(firingDirection, movementController, team); break; 
            case 1: SpikeSkillShot(firingDirection, movementController, team); break; 
            case 2: ExplodingSkillShot(firingDirection, movementController, team); break;
        } 
        currentSpike = (currentSpike + 1) % 3; 
        return true;
    }

    private void SpikeSkillShot(Vector3 firingDirection, MovementController movementController, Team team)
    {
        movementController.SetImmobilized(true, "BloodSpikesAttack");
        parentController.SetSkillsDisabled(true, "loodSpikesAttack");

        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 1f * firingDirection,
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[0],
            Lifetime = _storedSkillData.ProjectileLifetime,
            Speed = _storedSkillData.ProjectileSpeed,
            Team = team,

            Target = firingDirection * _storedSkillData.ProjectileRange, // Arbitrary long distance in the firing direction
        };

        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
        });

        projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset

        var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
        p.SetTravelMode(_storedSkillData.TravelMode);
    }

    private void ExplodingSkillShot(Vector3 firingDirection, MovementController movementController, Team team)
    {
        movementController.SetImmobilized(true, "BloodSpikesAttack");
        parentController.SetSkillsDisabled(true, "loodSpikesAttack");

        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 1f * firingDirection,
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[1], //The explosion will do the damage, so we used reduced or 0 dmg for the first projectile
            Lifetime = _storedSkillData.ProjectileLifetime,
            Speed = _storedSkillData.ProjectileSpeed,
            Team = team,

            Target = firingDirection * _storedSkillData.ProjectileRange, // Arbitrary long distance in the firing direction
        };

        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
        });

        projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset

        var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
        p.onProjectileHit.AddListener(Explosion);
        p.SetTravelMode(_storedSkillData.TravelMode);

        PutInCooldown();
    }

    private void Explosion(Projectile projectile, DamageController damageController)
    {
        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = damageController.transform.position,
            origin = damageController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[2],
            Lifetime = _storedSkillData.ProjectileLifetime,
            Team = projectile.Team,
        };

        SpawnProjectile(projectileData, 1);
        projectile.onProjectileHit.RemoveAllListeners();
    }
}

