using DG.Tweening;

public class DashStrategy: SkillStrategy
{
    public override void Initialize(SkillsController parent, SkillData skillData, PilotComponent pilot)
    {
        base.Initialize(parent, skillData, null);
    }

	public override bool Call(MovementController movementController, Team team)
	{
        if (!base.Call(movementController, team)) return false;

        if (_storedSkillData.ProvideInvulnerability)
        {
            var dc = movementController.GetComponent<DamageController>();
            
            dc.IsInvincible = true;

            DOVirtual.DelayedCall(_storedSkillData.InvulnerabilityTime, () => dc.IsInvincible = false);
        }
        
		movementController.AnimController?.Trigger(_storedSkillData.AnimationKey);
        movementController.Dash(movementController.GetFacingDirection(), _storedSkillData.movementDistance, _storedSkillData.movementDuration, _storedSkillData.ignoreCollisions);
        PutInCooldown();
        return true;
    }
}
