using UnityEngine;
using System.Collections;


public class BloodwaveStrategy: SkillStrategy
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
		ProjectileData[] projectiles = new ProjectileData[_storedSkillData.numberOfProjectiles];
		for (int i = 0; i < _storedSkillData.numberOfProjectiles; i++)
		{
            ProjectileData projectileData = new ProjectileData()
            {
                startingPosition = movementController.transform.position + 1.5f * (Quaternion.AngleAxis(i * (360f / _storedSkillData.numberOfProjectiles), Vector3.up) * movementController.GetFacingDirection()),
                origin = movementController.transform.position,
                Damage = _storedSkillData.ProjectileDamage
            };

			projectileData.startingPosition += new Vector3(0, 1f, 0f); // Vertical Offset
			projectiles[i] = projectileData;
        }
        // Implement bloodwave logic here
        
		foreach(ProjectileData projectileData in projectiles)
		{
            StaticProjectile bloodwaveProjectile = Instantiate(projectilePrefab).GetComponent<StaticProjectile>();
			if (bloodwaveProjectile != null)
			{
				bloodwaveProjectile.Initialize(projectileData);
			}
			else
			{
				Debug.LogError("BloodwaveProjectile component not found on the projectile prefab.");
            }
        }

        Debug.Log("Executing Bloodwave Skill");
		PutInCooldown();
    }

}
