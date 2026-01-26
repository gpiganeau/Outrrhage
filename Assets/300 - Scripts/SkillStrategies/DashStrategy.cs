using UnityEngine;
using System.Collections;

public class DashStrategy: SkillStrategy
{
    public override void Initialize(SkillsController parent, SkillData skillData)
    {
        base.Initialize(parent, skillData);
    }

	public override bool Call(MovementController movementController)
	{
        if (!base.Call(movementController)) return false;
        
        movementController.Dash(movementController.GetFacingDirection(), _storedSkillData.movementDistance, _storedSkillData.movementDuration, _storedSkillData.ignoreCollisions);
        PutInCooldown();
        return true;
    }
}
