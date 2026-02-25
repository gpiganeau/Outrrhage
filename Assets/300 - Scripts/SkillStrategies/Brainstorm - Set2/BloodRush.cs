class BloodRush : SkillStrategy
{
    public override bool Call(MovementController movementController, Team team)
    {
        if (!base.Call(movementController, team)) return false;

        // -- Pensez vous qu'il va exister ?

        return true;
    }
}

