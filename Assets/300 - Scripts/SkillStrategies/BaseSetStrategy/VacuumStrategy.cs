using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

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
        var p = Instantiate(_storedSkillData.SkillProjectilePrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

        var r = _storedSkillData.Radius;
        p.transform.localScale = new Vector3(r,r,r);
        Collider[] colliders = Physics.OverlapSphere(riel.transform.position, r);

        foreach (Collider c in colliders)
        {
            if (c.TryGetComponent<BloodDrop>(out var drop)) drop.Attract(riel.gameObject);
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
