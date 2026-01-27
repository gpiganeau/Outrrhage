using DG.Tweening;

public class VacuumStrategy: SkillStrategy
{
    public override bool Call(MovementController movementController)
    {
        if (!base.Call(movementController)) return false;

        movementController.SetImmobilized(true, "Vacuum");
        parentController.SetSkillsDisabled(true, "Vacuum");

        var riel = GameManager.Instance.Riel;
        var drops = FindObjectsByType<BloodDrop>(UnityEngine.FindObjectsSortMode.None);

        foreach (BloodDrop d in drops)
        {
            d.Attract(riel.gameObject);
        }
        
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "Vacuum");
        });
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
        {
            movementController.SetImmobilized(false, "Vacuum");
        });
        PutInCooldown();
        return true;
    }
}
