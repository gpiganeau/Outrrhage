using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SlashStrategy: SkillStrategy
{
    public override bool Call(MovementController movementController)
    {
        if (!base.Call(movementController)) return false;

        ProjectileData projectileData = new ProjectileData() { 
            startingPosition = movementController.transform.position + 2f * movementController.GetFacingDirection(), 
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage,
            Lifetime = _storedSkillData.ProjectileLifetime
        };

        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");
        SpawnProjectile(projectileData);

        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseMinTimeBetweenSkills, () =>
        {
            parentController.SetSkillsDisabled(false, "SlashAttack");
        });
        DOVirtual.DelayedCall(SettingsManager.Instance.GameplaySettings.baseStaticTimeOnSkillUse, () =>
        {
            movementController.SetImmobilized(false, "SlashAttack");
        });
        PutInCooldown();
        return true;
    }
}
