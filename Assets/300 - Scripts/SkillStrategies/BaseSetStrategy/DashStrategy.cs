using UnityEngine;
using System.Collections;

public class DashStrategy: SkillStrategy
{
    public override void Initialize(SkillsController parent, SkillData skillData)
    {
        base.Initialize(parent, skillData);
    }

	public override bool Call(MovementController movementController, Team team)
	{
        if (!base.Call(movementController, team)) return false;
        
        movementController.Dash(movementController.GetFacingDirection(), _storedSkillData.movementDistance, _storedSkillData.movementDuration, _storedSkillData.ignoreCollisions);
        if (team == Team.Ally) Juicer.I.SlowMotion(0.4f, _storedSkillData.movementDuration); // -- Only Riel Slow Motion
        PutInCooldown();
        return true;
    }
}
