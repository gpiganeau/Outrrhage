using UnityEngine.UIElements;

public class SettingsPanelGameplay : SettingsPanelBase
{
    public SettingsPanelGameplay(VisualElement root, SettingsManager manager)
        : base(root, manager) { }

    protected override void Populate()
    {
        var s = Manager.GameplaySettings;
        if (s == null) return;

        SetSlider("gp-skill-static-time",   s.baseStaticTimeOnSkillUse);
        SetSlider("gp-skill-min-delay",     s.baseMinTimeBetweenSkills);
        SetSlider("gp-spawner-delay",       s.spawnerTimeBetweenSpawns);
        SetSlider("gp-death-reload-delay",  s.DeathTimeBeforeReload);
        SetToggle("gp-clear-on-death",      s.ClearRoomOnDeath);
    }

    public override void Apply()
    {
        var s = Manager.GameplaySettings;
        if (s == null) return;

        s.baseStaticTimeOnSkillUse = GetSlider("gp-skill-static-time");
        s.baseMinTimeBetweenSkills = GetSlider("gp-skill-min-delay");
        s.spawnerTimeBetweenSpawns = GetSlider("gp-spawner-delay");
        s.DeathTimeBeforeReload    = GetSlider("gp-death-reload-delay");
        s.ClearRoomOnDeath         = GetToggle("gp-clear-on-death");
    }

    public override void Reset() => Populate();
}
