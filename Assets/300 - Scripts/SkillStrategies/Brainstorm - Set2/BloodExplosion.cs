using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Text;

class BloodExplosion : SkillStrategy
{
    Sequence channelSequence;
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        movementController.SetImmobilized(true, "SlashAttack");
        parentController.SetSkillsDisabled(true, "SlashAttack");

        channelSequence = DOTween.Sequence();
        channelSequence.AppendInterval(_storedSkillData.HoldDuration);
        channelSequence.OnComplete(() =>
        {
            ExecuteExplosion();
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
        });

        

        return true;
    }

    private void ExecuteExplosion()
    {

        // Logic to damage nearby enemies based on the amount of blood consumed during the channeling
    }

    public override void Release(MovementController movementController, Team team)
    {
        base.Release(movementController, team);

        if (channelSequence != null) 
        {
            channelSequence.Kill();
            parentController.SetSkillsDisabled(false, "SlashAttack");
            movementController.SetImmobilized(false, "SlashAttack");
        }
    }
}

