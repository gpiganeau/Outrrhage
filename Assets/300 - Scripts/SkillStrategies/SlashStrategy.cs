using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SlashStrategy: SkillStrategy
{
    public override void Call(MovementController movementController)
    {
        ProjectileData projectileData = new ProjectileData() { 
            startingPosition = movementController.transform.position + 1.5f * movementController.GetFacingDirection(), 
            origin = movementController.transform.position,
            Damage = _storedSkillData.ProjectileDamage
        };

        if (isInCooldown) return;
        base.Call(movementController);
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
    }

}
