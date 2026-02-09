using DG.Tweening;

public class HealStrategy: SkillStrategy
{
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        const string strategyTag = "Heal";
        movementController.SetImmobilized(true, strategyTag);
        parentController.SetSkillsDisabled(true, strategyTag);

        // -- Heal Implementation -- //
        if (movementController.TryGetComponent<DamageController>(out var dc))
        {
            dc.Heal(_storedSkillData.ProjectileDamage); // Use Damage for Heal Amount... - Also maybe we should spawna  projectile, that trigger with ourselve ? Idk.
            Juicer.I.PlayerHealedEffect(null); // -- Need a way to pass correctly all types of Renderer [Manager ?] @TODO
        }
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
