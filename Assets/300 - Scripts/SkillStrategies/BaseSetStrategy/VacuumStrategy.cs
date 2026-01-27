using DG.Tweening;

public class VacuumStrategy: SkillStrategy
{
    public override bool Call(MovementController movementController)
    {
        if (!base.Call(movementController)) return false;

        const string strategyTag = "Vacuum";
        movementController.SetImmobilized(true, strategyTag);
        parentController.SetSkillsDisabled(true, strategyTag);

        // -- Vacuum Implementation -- //
        var riel = GameManager.Instance.Riel;
        var drops = FindObjectsByType<BloodDrop>(UnityEngine.FindObjectsSortMode.None);

        foreach (BloodDrop d in drops)
        {
            d.Attract(riel.gameObject);
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
