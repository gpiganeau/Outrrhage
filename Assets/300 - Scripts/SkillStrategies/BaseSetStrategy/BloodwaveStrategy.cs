using UnityEngine;
using System.Collections;
using DG.Tweening;


public class BloodwaveStrategy: SkillStrategy
{
	public override bool Call(MovementController movementController, Team team)
	{
        if (!base.Call(movementController, team)) return false;

		ProjectileData[] projectiles = new ProjectileData[_storedSkillData.numberOfProjectiles];
		for (int i = 0; i < _storedSkillData.numberOfProjectiles; i++)
		{
            ProjectileData projectileData = new ProjectileData()
            {
                startingPosition = movementController.transform.position + 1.5f * (Quaternion.AngleAxis(i * (360f / _storedSkillData.numberOfProjectiles), Vector3.up) * movementController.GetFacingDirection()),
                origin = movementController.transform.position,
                Damage = _storedSkillData.ProjectileDamage[0],
				Range = _storedSkillData.ProjectileRange,
                Lifetime = 2 * _storedSkillData.ProjectileRange / _storedSkillData.ProjectileSpeed,
				Speed = _storedSkillData.ProjectileSpeed,
				BloodStackingAmount = _storedSkillData.BloodStackingAmount,
                Target = new Vector3(Random.Range(-20, 20) , 0, Random.Range(-20, 20)),
				Team = team,
            };

			projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset
			projectiles[i] = projectileData;

			// -- Animation
			movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);
			Camera.main.transform.DOShakePosition(
				_storedSkillData.ShakeDuration,
				_storedSkillData.ShakeStrength,
				_storedSkillData.ShakeVibrato,
				_storedSkillData.ShakeRandomness,
				false,
				true
			);

			// -- Blood Wave Logic -- 
			var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
			p.SetTravelMode(_storedSkillData.TravelMode);
			p.onProjectileHit.AddListener(StackBlood);
        	//DOVirtual.DelayedCall(projectileData.Lifetime * 0.5f, () => p.SetTravelMode(SkillshotProjectile.TravelMode.TowardCaster));
        }

		PutInCooldown();
		return true;
    }

    protected override void OnProjectileHit(Projectile projectile, DamageController damageController)
    {
        base.OnProjectileHit(projectile, damageController);
		StackBlood(projectile, damageController);
    }
}
