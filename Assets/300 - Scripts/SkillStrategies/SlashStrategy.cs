using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SlashStrategy: SkillStrategy
{
    public override void Call(MovementController movementController)
    {
        base.Call(movementController);
        movementController.SetImmobilized(true, "SlashAttack");
        SpawnProjectile();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            movementController.SetImmobilized(false, "SlashAttack");
        });
    }

    private void SpawnProjectile()
    {

    }

}
