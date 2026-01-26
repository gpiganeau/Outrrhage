using UnityEngine;
using System.Collections;
using DG.Tweening;


public class DroneProjectileStrategy: SkillStrategy
{
	public override bool Call(MovementController movementController)
	{
        if (!base.Call(movementController)) return false;

		ProjectileData projectileData = new ProjectileData()
		{
			startingPosition = movementController.transform.position + 1.5f * movementController.GetFacingDirection(),
			origin = movementController.transform.position,
			Damage = _storedSkillData.ProjectileDamage,
			Lifetime = _storedSkillData.ProjectileLifetime,
			Speed = _storedSkillData.ProjectileSpeed,

			Target = new Vector3(Random.Range(-20, 20) , 0, Random.Range(-20, 20))
		};

		projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset

		var p = SpawnProjectile(projectileData) as SkillshotProjectile;
		p.SetTravelMode(_storedSkillData.TravelMode);

		PutInCooldown();
	
		return true;
	}
}
