using UnityEngine;
using System.Collections;
using DG.Tweening;


public class DroneProjectileStrategy: SkillStrategy
{
	public override bool Call(MovementController movementController, Team team)
	{
        if (!base.Call(movementController, team)) return false;

		ProjectileData projectileData = new ProjectileData()
		{
			startingPosition = movementController.transform.position + 1.5f * movementController.GetFacingDirection(),
			origin = movementController.transform.position,
			Damage = _storedSkillData.ProjectileDamage[0],
			Lifetime = _storedSkillData.ProjectileLifetime,
			Speed = _storedSkillData.ProjectileSpeed,
			Team = team,

			Target = new Vector3(Random.Range(-20, 20) , 0, Random.Range(-20, 20))
		};

		projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset
		movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);


		var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
		p.SetTravelMode(_storedSkillData.TravelMode);

		PutInCooldown();
	
		return true;
	}
}
