using UnityEngine;
using System.Collections;
using DG.Tweening;


public class DroneProjectileStrategy: SkillStrategy
{
	public override void Call(MovementController movementController)
	{
		if(isInCooldown)
		{
			Debug.Log("Skill is in cooldown.");
			return;
		}
		if (movementController == null)
		{
			Debug.LogError("MovementController is null.");
			return;
		}

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

        //Debug.Log("Executing Bloodwave Skill");
		PutInCooldown();
    }
}
