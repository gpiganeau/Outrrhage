using UnityEngine;
using System.Collections;
using DG.Tweening;


public class TourelleProjectileStrategy: SkillStrategy
{
	public override bool Call(MovementController movementController, Team team)
	{
        if (!base.Call(movementController, team))
			return false;

		int numberOfShots = 3;
        float delayBetweenShots = 0.2f;
		Vector3 firingDirection = movementController.GetFacingDirection();
        UseAimAssist(ref firingDirection, _storedSkillData.AimAssistRatio, team);

        for (int i = 0; i < numberOfShots; i++)
        {
            DOVirtual.DelayedCall(delayBetweenShots * i, () =>
            {

				ProjectileData projectileData = new ProjectileData()
				{
					startingPosition = movementController.transform.position + 1.5f * movementController.GetFacingDirection(),
					origin = movementController.transform.position,
					Damage = _storedSkillData.ProjectileDamage[0],
					Lifetime = _storedSkillData.ProjectileLifetime,
					Speed = _storedSkillData.ProjectileSpeed,
					Team = team,

					Target = movementController.transform.position + firingDirection * _storedSkillData.ProjectileRange,
				};

		projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset
		movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);

		_vfxController.PlayCastVFX(movementController.transform);

		var p = SpawnProjectile(projectileData, 0) as SkillshotProjectile;
		p.SetTravelMode(_storedSkillData.TravelMode);
		});
	}

		PutInCooldown();
	
		return true;
	}
}
