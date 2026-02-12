using System;
using System.Collections.Generic;
using System.Text;

class BloodRush : SkillStrategy
{
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;



        return true;
    }
}

