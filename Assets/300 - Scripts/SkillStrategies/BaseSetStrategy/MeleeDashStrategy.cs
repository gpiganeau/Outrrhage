using UnityEngine;
using System.Collections;

public class MeleeDashStrategy: SkillStrategy
{
    public override void Initialize(SkillsController parent, SkillData skillData)
    {
        base.Initialize(parent, skillData);
    }

	public override bool Call(MovementController movementController, Team team)
	{
        if (!base.Call(movementController, team)) return false;
        
		movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);
        movementController.Dash(movementController.GetFacingDirection(), _storedSkillData.movementDistance, _storedSkillData.movementDuration, _storedSkillData.ignoreCollisions);
        PutInCooldown();
        return true;
    }
}
