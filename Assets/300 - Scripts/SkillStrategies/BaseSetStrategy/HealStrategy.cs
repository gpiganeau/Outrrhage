using DG.Tweening;

public class HealStrategy: SkillStrategy
{
    public override bool Call(MovementController movementController)
    {
        if (!base.Call(movementController)) return false;

        const string strategyTag = "Heal";
        movementController.SetImmobilized(true, strategyTag);
        parentController.SetSkillsDisabled(true, strategyTag);

        // -- Heal Implementation -- //
        var riel = GameManager.Instance.Riel;
        // -------------------------- 

        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, strategyTag);
        });
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
        {
            movementController.SetImmobilized(false, strategyTag);
        });
        PutInCooldown();
        return true;
    }
}
