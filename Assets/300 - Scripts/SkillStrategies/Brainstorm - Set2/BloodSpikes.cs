using DG.Tweening;
using UnityEngine;

class BloodSpikes : SkillStrategy
{
    int currentSpike = 0;
    Tween comboResetTimer;

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
        return true;
    }

    private void SpikeSkillShot(Vector3 firingDirection, MovementController movementController, Team team)
    {
        movementController.SetImmobilized(true, "BloodSpikesAttack");
        parentController.SetSkillsDisabled(true, "BloodSpikesAttack");

        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + firingDirection,
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[0],
            Lifetime = _storedSkillData.ProjectileRange / _storedSkillData.ProjectileSpeed,
            Speed = _storedSkillData.ProjectileSpeed,
            BloodStackingAmount = _storedSkillData.BloodStackingAmount,
            Team = team,
            Target = movementController.transform.position + firingDirection * _storedSkillData.ProjectileRange, 
        };

        projectileData.startingPosition += new Vector3(0, 0.5f, 0f); // Vertical Offset

        var Seq = DOTween.Sequence();
        AudioManager.Instance.PlayClipAtPoint(_storedSkillData.CustomClips[currentSpike], transform.position);
        Seq.AppendCallback(() => movementController.AnimController?.Trigger(_storedSkillData.AnimationsKeys[currentSpike]));
        Seq.AppendInterval(0.125f);
        Seq.AppendCallback(() => {
             var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
             p.SetTravelMode(_storedSkillData.TravelMode);
            currentSpike = (currentSpike + 1) % 3;
            parentController.SetSkillsDisabled(false, "BloodSpikesAttack");
            movementController.SetImmobilized(false, "BloodSpikesAttack");
            ResetComboAfterDelay();
            CustomCooldown();
        });

    }

    private void CustomCooldown()
    {
        isInCooldown = true;
        DOVirtual.DelayedCall(0.2f, () => isInCooldown = false);
    }

    private void ExplodingSkillShot(Vector3 firingDirection, MovementController movementController, Team team)
    {
        movementController.SetImmobilized(true, "BloodSpikesAttack");
        parentController.SetSkillsDisabled(true, "BloodSpikesAttack");

        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = movementController.transform.position + 1f * firingDirection,
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[1], //The explosion will do the damage, so we used reduced or 0 dmg for the first projectile
            Lifetime = _storedSkillData.ProjectileLifetime,
            Speed = _storedSkillData.ProjectileSpeed,
            BloodStackingAmount = 0, // No blood stacking on the initial projectile, only on the explosion
            Team = team,

            Target = movementController.transform.position + firingDirection * _storedSkillData.ProjectileRange,
        };

        projectileData.startingPosition += new Vector3(0, 0.5f, 0f); // Vertical Offset

        var Seq = DOTween.Sequence();
        Seq.AppendCallback(() => movementController.AnimController?.Trigger(_storedSkillData.AnimationsKeys[currentSpike]));
            AudioManager.Instance.PlayClipAtPoint(_storedSkillData.CustomClips[2], transform.position);
        Seq.AppendInterval(0.85f * 0.5f);
        Seq.AppendCallback(() => {
            var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
            p.onProjectileHit.AddListener(Explosion);
            p.SetTravelMode(_storedSkillData.TravelMode);
            currentSpike = (currentSpike + 1) % 3;
            base.PutInCooldown();   
            parentController.SetSkillsDisabled(false, "BloodSpikesAttack");
            movementController.SetImmobilized(false, "BloodSpikesAttack");
        });

    }

    
    private void ResetComboAfterDelay()
    {
        comboResetTimer?.Kill();
        comboResetTimer = DOVirtual.DelayedCall(_storedSkillData.ComboResetDelay, () =>
        {
            currentSpike = 0;
        });
    }

    private void Explosion(Projectile projectile, DamageController damageController)
    {
        ProjectileData projectileData = new ProjectileData()
        {
            startingPosition = damageController.transform.position,
            origin = damageController.transform.position,
            Damage = _storedSkillData.ProjectileDamage[2],
            Lifetime = 0.3f,
            BloodStackingAmount = _storedSkillData.BloodStackingAmount,
            Team = projectile.Data.Team,
        };

        SpawnProjectile(projectileData, 1);
        projectile.onProjectileHit.RemoveAllListeners();
    }

    protected override void OnProjectileHit(Projectile projectile, DamageController damageController)
    {
        base.OnProjectileHit(projectile, damageController);
        StackBlood(projectile, damageController);
    }
}

