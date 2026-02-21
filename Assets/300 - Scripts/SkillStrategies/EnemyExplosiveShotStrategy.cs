using DG.Tweening;
using UnityEngine;

class EnemyExplosiveShotStrategy : SkillStrategy
{
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) 
            return false;

        Vector3 firingDirection = movementController.GetFacingDirection();
        UseAimAssist(ref firingDirection, _storedSkillData.AimAssistRatio, team);

        movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);
        
        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 1f * firingDirection,
            origin = movementController.transform.position,
            Damage = 0, // dégâts appliqués uniquement par l'explosion
            Lifetime = _storedSkillData.ProjectileLifetime,
            Speed = _storedSkillData.ProjectileSpeed,
            BloodStackingAmount = 0,
            Team = team,
            Target = movementController.transform.position 
                     + firingDirection * _storedSkillData.ProjectileRange,
        };

        projectileData.startingPosition += new Vector3(0, 0.5f, 0f);

        var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
        p.onProjectileHit.AddListener(Explosion);
        p.SetTravelMode(_storedSkillData.TravelMode);

        PutInCooldown();
        return true;
    }

    private void Explosion(Projectile projectile, DamageController damageController)
    {
        ProjectileData explosionData = new ProjectileData()
        {
            startingPosition = damageController.transform.position,
            origin = damageController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[2],
            Lifetime = 0.3f,
            BloodStackingAmount = _storedSkillData.BloodStackingAmount,
            Team = projectile.Data.Team,
        };

        SpawnProjectile(explosionData, 1);

        projectile.onProjectileHit.RemoveAllListeners();
    }

    protected override void OnProjectileHit(Projectile projectile, DamageController damageController)
    {
        base.OnProjectileHit(projectile, damageController);
        StackBlood(projectile, damageController);
    }
}