using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SlashStrategy: SkillStrategy
{
    public override void Call(MovementController movementController)
    {
        if (isInCooldown) return;
        base.Call(movementController);
        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");
        SpawnProjectile(movementController.transform.position + 2f * movementController.GetFacingDirection());
        DOVirtual.DelayedCall(0.2f, () =>
        {
            movementController.SetImmobilized(false, "SlashAttack");
            parentController.SetSkillsDisabled(false, "SlashAttack");
        });
        PutInCooldown();
    }

}
