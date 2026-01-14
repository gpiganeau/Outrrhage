using UnityEngine;
using System.Collections;

public class DashStrategy: SkillStrategy
{
    public override void Initialize(SkillsController parent, SkillData skillData)
    {
        base.Initialize(parent, skillData);
    }

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
        movementController.Dash(movementController.GetFacingDirection(), _storedSkillData.movementDistance, _storedSkillData.movementDuration, _storedSkillData.ignoreCollisions);
        // Implement dash logic here
        Debug.Log("Executing Dash Skill");
        PutInCooldown();
    }
}
