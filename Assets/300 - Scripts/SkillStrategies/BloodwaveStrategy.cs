using UnityEngine;
using System.Collections;
using DG.Tweening;


public class BloodwaveStrategy: SkillStrategy
{
	public override bool Call(MovementController movementController)
	{
		if(isInCooldown)
		{
			//Logger.Combat("Skill is in cooldown.");
			return false;
		}
		if (movementController == null)
		{
			Logger.LogError(Logger.LogCategory.Core, "MovementController is null");
			return false;
		}
		ProjectileData[] projectiles = new ProjectileData[_storedSkillData.numberOfProjectiles];
		for (int i = 0; i < _storedSkillData.numberOfProjectiles; i++)
		{
            ProjectileData projectileData = new ProjectileData()
            {
                startingPosition = movementController.transform.position + 1.5f * (Quaternion.AngleAxis(i * (360f / _storedSkillData.numberOfProjectiles), Vector3.up) * movementController.GetFacingDirection()),
                origin = movementController.transform.position,
                Damage = _storedSkillData.ProjectileDamage,
				Lifetime = _storedSkillData.ProjectileLifetime,
				Speed = _storedSkillData.ProjectileSpeed,
				Target = new Vector3(Random.Range(-20, 20) , 0, Random.Range(-20, 20))
            };

			projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset
			projectiles[i] = projectileData;

			// -- Blood Wave Logic -- 
			var p = SpawnProjectile(projectileData) as SkillshotProjectile;
			p.SetTravelMode(_storedSkillData.TravelMode);
        	DOVirtual.DelayedCall(projectileData.Lifetime * 0.5f, () => p.SetTravelMode(SkillshotProjectile.TravelMode.TowardCaster));
        }

		PutInCooldown();
		return true;
    }
}
